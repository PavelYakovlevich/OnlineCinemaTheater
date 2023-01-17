using AutoMapper;
using Dapper;
using MediaInfo.Contract.Repositories;
using MediaInfo.Data.Entities;
using MediaInfo.Domain.MediaInfo;
using Npgsql;
using System.Data.Common;
using System.Globalization;
using static Dapper.SqlMapper;

namespace MediaInfo.Data.Repositories;

public class MediaInfoRepository : IMediaInfoRepository
{
    private const string MediaInfoTable = "Medias";
    private const string MediasParticipantsTable = "Participants_Medias";
    private const string MediasGenresTable = "Genres_Medias";
    private const string GenresTable = "Genres";
    private const string ParticipantsTable = "Participants";

    private readonly string _connectionString;
    private readonly IMapper _mapper;

    public MediaInfoRepository(string connectionString, IMapper mapper)
    {
        _connectionString = connectionString;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(MediaInfoModel model)
    {
        var entity = _mapper.Map<MediaInformation>(model);

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        var mediaId = await connection.QueryFirstAsync<Guid>($@"
            INSERT INTO ""{MediaInfoTable}""
            (name, budget, aid, description, ""issueDate"", ""isFree"", ""ageRating"", ""isTVSerias"", ""isVisible"", country)
            VALUES
            (@name, @budget, @aid, @description, @issueDate, @isfree, @agerating, @istvserias, @isvisible, @country)
            RETURNING id;", 
            entity);

        await CreateMediaParticipants(connection, mediaId, model.Participants.Select(p => p.Id));

        await CreateMediaGenres(connection, mediaId, model.Genres.Select(p => p.Id));

        await transaction.CommitAsync();

        return mediaId;
    }

    public async Task<MediaInfoModel> ReadByIdAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var medias = await connection.QueryAsync<MediaInformation, Participant, Genre, MediaInformation>($@"
            SELECT * 
            FROM ""{MediaInfoTable}"" AS MI
            INNER JOIN ""{MediasParticipantsTable}"" AS MP
            ON MP.""mediaId"" = MI.id
            INNER JOIN ""{ParticipantsTable}"" AS P
            ON P.id = MP.""participantId""
            INNER JOIN ""{MediasGenresTable}"" AS MG
            ON MG.""mediaId"" = MI.id
            INNER JOIN ""{GenresTable}"" AS G
            ON G.id = MG.""genreId""
            WHERE MI.Id = '{id}';", (mediaInfo, participant, genre) =>
        {
            mediaInfo.Participants = new List<Participant> { participant };
            mediaInfo.Genres = new List<Genre> { genre };

            return mediaInfo;
        });

        var result = medias.GroupBy(m => m.Id)
            .Select(g =>
            {
                var media = g.First();

                media.Participants = g.Select(g => g.Participants.Single()).DistinctBy(p => p.Id).ToArray();
                media.Genres = g.Select(g => g.Genres.Single()).DistinctBy(g => g.Id).ToArray();

                return media;
            })
            .FirstOrDefault();

        return _mapper.Map<MediaInfoModel>(result);
    }

    public async IAsyncEnumerable<MediaInfoModel> ReadAllAsync(MediaInfoFiltersModel filters)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var innerFilterStatement = GetInnerSqlFilterStatement(filters);
        var outterFilterStatement = GetOutterSqlFilterStatement(filters.Genres.ToArray());

        var medias = await connection.QueryAsync<MediaInformation, Participant, Genre, MediaInformation>($@"
            SELECT *
            FROM 
            (
                SELECT *
                FROM
                (
                    SELECT MI.*, ARRAY_AGG(G.""name"") AS media_genres
                    FROM ""{MediaInfoTable}"" AS MI
                    INNER JOIN ""{MediasGenresTable}"" AS MG
                    ON MG.""mediaId"" = MI.id
                    INNER JOIN ""{GenresTable}"" AS G
                    ON G.id = MG.""genreId""
                    {innerFilterStatement}
                    GROUP BY (MI.id)
                ) AS MI
                {outterFilterStatement}
                ORDER BY (MI.""issueDate"") DESC
                OFFSET {filters.Offset}
                LIMIT {filters.Limit}

            ) AS MI
            INNER JOIN ""{MediasParticipantsTable}"" AS MP
            ON MP.""mediaId"" = MI.id
            INNER JOIN ""{ParticipantsTable}"" AS P
            ON P.id = MP.""participantId""
            INNER JOIN ""{MediasGenresTable}"" AS MG
            ON MG.""mediaId"" = MI.id
            INNER JOIN ""{GenresTable}"" AS G
            ON G.id = MG.""genreId""
            ", (mi, p, g) =>
        {
            mi.Participants = new List<Participant> { p };
            mi.Genres = new List<Genre> { g };

            return mi;
        }, splitOn: "id");

        var result = medias.GroupBy(m => m.Id)
            .Select(g =>
            {
                var media = g.First();

                media.Participants = g.Select(g => g.Participants.Single()).DistinctBy(p => p.Id).ToArray();
                media.Genres = g.Select(g => g.Genres.Single()).DistinctBy(g => g.Id).ToArray();

                return media;
            });

        foreach (var mediaInfo in result)
        {
            yield return _mapper.Map<MediaInfoModel>(mediaInfo);
        }

        static string GetOutterSqlFilterStatement(string[] genres)
        {
            if (genres.Length == 0)
            {
                return string.Empty;
            }

            var genresSqlArray = $"array[{string.Join(',', genres.Select(g => $"'{g}'::varchar(50)"))}]";
            return @$"WHERE MI.""media_genres"" @> { genresSqlArray }";
        }

        static string GetInnerSqlFilterStatement(MediaInfoFiltersModel filters, string mediaInfoTable = "MI", string genreTable = "G") =>
            @$"WHERE {mediaInfoTable}.""name"" LIKE '{filters.NameStartsWith ?? string.Empty}%'" +
            @$"{(filters.IsTvSerias is not null ? @$" AND {mediaInfoTable}.""isTVSerias"" = {filters.IsTvSerias}" : string.Empty)}" +
            $"{(filters.Country is not null ? @$" AND {mediaInfoTable}.""country"" = '{filters.Country}'" : string.Empty)}";
    }

    public async Task<bool> UpdateAsync(Guid id, PartialUpdateMediaInfoModel model)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        var rowsAffectedCount = await connection.ExecuteAsync($@"
            UPDATE ""{MediaInfoTable}""
            SET
                name           = '{model.Name}',
                budget         = {(model.Budget is null ? "null" : model.Budget.Value.ToString(CultureInfo.InvariantCulture))},
                aid            = {(model.Aid is null ? "null" : model.Aid.Value.ToString(CultureInfo.InvariantCulture))},
                description    = '{model.Description}',
                ""issueDate""  = '{model.IssueDate:yyyy/MM/dd}',
                ""isFree""     = {model.IsFree},
                ""ageRating""  = {(int)model.AgeRating},
                ""isTVSerias"" = {model.IsTvSerias},
                ""isVisible""  = {model.IsVisible},
                country        = '{model.Country}'
            WHERE Id = '{id}';
        ");

        if (rowsAffectedCount != 0)
        {
            await UpdateMediaRelatedData(connection, id, model);
        }

        await transaction.CommitAsync();

        return rowsAffectedCount != 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        await connection.ExecuteAsync($@"
            DELETE FROM ""{MediasGenresTable}""
            WHERE ""mediaId"" = '{id}'");

        await connection.ExecuteAsync($@"
            DELETE FROM ""{MediasParticipantsTable}""
            WHERE ""mediaId"" = '{id}'");

        var rowsAffected = await connection.ExecuteAsync($@"
            DELETE FROM ""{MediaInfoTable}""
            WHERE id = '{id}'");

        await transaction.CommitAsync();

        return rowsAffected != 0;
    }

    private static async Task CreateMediaParticipants(DbConnection connection, Guid mediaId, IEnumerable<Guid> ids)
    {
        var mediaParticipantsValuesString = string.Join(',', ids
            .Select(id => $"('{mediaId}','{id}')")
            .ToArray());

        await connection.ExecuteAsync($@"
            INSERT INTO ""{MediasParticipantsTable}""
            (""mediaId"", ""participantId"")
            VALUES
            {mediaParticipantsValuesString}");
    }

    private static async Task CreateMediaGenres(DbConnection connection, Guid mediaId, IEnumerable<Guid> ids)
    {
        var mediaGenresValuesString = string.Join(',', ids
            .Select(id => $"('{id}','{mediaId}')"));

        await connection.ExecuteAsync($@"
            INSERT INTO ""{MediasGenresTable}""
            (""genreId"", ""mediaId"")
            VALUES
            {mediaGenresValuesString}");
    }

    private static async Task UpdateMediaRelatedData(NpgsqlConnection connection, Guid id, PartialUpdateMediaInfoModel model)
    {
        if (model.Participants.Any())
        {
            await DeleteMediaParticipants(connection, id);
            await CreateMediaParticipants(connection, id, model.Participants);
        }

        if (model.Genres.Any())
        {
            await DeleteMediaGenres(connection, id);
            await CreateMediaGenres(connection, id, model.Genres);
        }
    }

    private static async Task DeleteMediaParticipants(DbConnection connection, Guid id)
    {
        await connection.ExecuteAsync($@"
            DELETE FROM ""{MediasParticipantsTable}""
            WHERE ""mediaId"" = '{id}';
        ");
    }

    private static async Task DeleteMediaGenres(DbConnection connection, Guid id)
    {
        await connection.ExecuteAsync($@"
            DELETE FROM ""{MediasGenresTable}""
            WHERE ""mediaId"" = '{id}';
        ");
    }
}
