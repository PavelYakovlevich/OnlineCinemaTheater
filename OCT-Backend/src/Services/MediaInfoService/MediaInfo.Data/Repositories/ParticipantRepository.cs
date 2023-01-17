using AutoMapper;
using Dapper;
using MediaInfo.Contract.Repositories;
using MediaInfo.Data.Entities;
using MediaInfo.Domain.Participant;
using Npgsql;

namespace MediaInfo.Data.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private const string ParticipantsTableName = "Participants";
    private readonly string _connectionString;
    private readonly IMapper _mapper;

    public ParticipantRepository(string connectionString, IMapper mapper)
    {
        _connectionString = connectionString;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(ParticipantModel model)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var entity = _mapper.Map<Participant>(model);

        return await connection.QueryFirstAsync<Guid>($@"
            INSERT INTO ""{ParticipantsTableName}""
            (name, surname, birthday, description, role, country)
            VALUES 
            (@name, @surname, @birthday, @description, @role, @country)
            RETURNING id;",
            entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var rowsAffrected = await connection.ExecuteAsync($@"
            DELETE FROM ""{ParticipantsTableName}"" WHERE Id = '{id}'");

        return rowsAffrected != 0;
    }

    public async IAsyncEnumerable<ParticipantModel> FindAllAsync(ParticipantFiltersModel filtersModel)
    {
        var condition = GetSqlFilterStatement(filtersModel);

        await using var connection = new NpgsqlConnection(_connectionString);

        var participants = await connection.QueryAsync<Participant>($@"
            SELECT * FROM ""{ParticipantsTableName}"" 
            WHERE {condition}
            LIMIT {filtersModel.Limit}
            OFFSET {filtersModel.Offset};");

        foreach (var participant in participants)
        {
            yield return _mapper.Map<ParticipantModel>(participant);
        }
    }

    public async Task<ParticipantModel> ReadById(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var entity = await connection.QueryFirstOrDefaultAsync<Participant>($@"
            SELECT * FROM ""{ParticipantsTableName}"" WHERE Id = '{id}'");

        return _mapper.Map<ParticipantModel>(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, ParticipantModel model)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var entity = _mapper.Map<Participant>(model);

        var rowsAffectedCount = await connection.ExecuteAsync($@"
            UPDATE ""{ParticipantsTableName}""
            SET 
                Name        = @name, 
                Surname     = @surname, 
                Birthday    = @birthday, 
                Description = @description, 
                Role        = @role,
                Country     = @country
            WHERE Id = '{id}'",
            entity);

        return rowsAffectedCount != 0;
    }

    private string GetSqlFilterStatement(ParticipantFiltersModel filtersModel)
    {
        var whereStatement = @$"Name LIKE '{filtersModel.NameStartsWith ?? string.Empty}%' 
                                AND Surname LIKE '{filtersModel.SurnameStartsWith ?? string.Empty}%'";

        if (filtersModel.Role is not null)
        {
            whereStatement += $"AND Role = '{(int)filtersModel.Role}'";
        }

        return whereStatement;
    }
}
