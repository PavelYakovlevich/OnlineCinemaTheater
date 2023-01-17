using Dapper;
using Npgsql;

namespace MediaInfo.Data;

public class Database
{
    private readonly string _connectionString;

    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task EnsureCreatedAsync(string name)
    {
        var query = "SELECT * FROM pg_catalog.pg_database WHERE lower(datname) = lower(@name);";
        var parameters = new DynamicParameters();
        parameters.Add("name", name);

        await using var connection = new NpgsqlConnection(_connectionString);

        if (!connection.Query(query, parameters).Any())
        {
            await connection.ExecuteAsync($"CREATE DATABASE {name};");
        }
    }
}
