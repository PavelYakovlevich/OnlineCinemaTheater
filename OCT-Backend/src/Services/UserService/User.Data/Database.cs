using Dapper;
using Microsoft.Data.SqlClient;

namespace User.Data;

public class Database
{
	private readonly string _connectionString;

	public Database(string connectionString)
	{
		_connectionString = connectionString;
	}

	public async Task EnsureCreatedAsync(string name)
	{
        var query = "SELECT * FROM sys.databases WHERE name = @name;";
        var parameters = new DynamicParameters();
        parameters.Add("name", name);

        await using var connection = new SqlConnection(_connectionString);

        if (!connection.Query(query, parameters).Any())
        {
            await connection.ExecuteAsync($"CREATE DATABASE {name};");
        }
    }
}
