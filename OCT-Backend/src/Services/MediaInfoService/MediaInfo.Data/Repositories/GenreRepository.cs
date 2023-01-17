using AutoMapper;
using Dapper;
using MediaInfo.Contract.Repositories;
using MediaInfo.Data.Entities;
using MediaInfo.Domain.Genre;
using Npgsql;

namespace MediaInfo.Data.Repositories;

public class GenreRepository : IGenreRepository
{
    private const string GenresTableName = "Genres";
    private readonly string _connectionString;
    private readonly IMapper _mapper;

    public GenreRepository(string connectionString, IMapper mapper)
    {
        _connectionString = connectionString;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(GenreModel genreModel)
    {
        var entity = _mapper.Map<Genre>(genreModel);

        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QueryFirstAsync<Guid>($@"
            INSERT INTO ""{GenresTableName}""
            (name)
            VALUES 
            (@name)
            RETURNING id;",
            entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var rowsAffrected = await connection.ExecuteAsync($@"
            DELETE FROM ""{GenresTableName}"" WHERE Id = '{id}'");

        return rowsAffrected != 0;
    }

    public async IAsyncEnumerable<GenreModel> ReadAllAsync(GenreFiltersModel filtersModel)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var genres = await connection.QueryAsync<Genre>($@"
            SELECT * FROM ""{GenresTableName}""
            WHERE Name LIKE '{filtersModel.NameStartsWith}%'");

        foreach (var genre in genres)
        {
            yield return _mapper.Map<GenreModel>(genre);
        }
    }
}
