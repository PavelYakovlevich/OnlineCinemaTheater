using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Comment.Data.Extensions;

public static class HostExtensions
{
    public static async Task EnsureDatabaseCreated(this IHost app, string dbName, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB");

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(dbName);

        var colletions = await database.ListCollectionNamesAsync();
        
        if (!await colletions.AnyAsync())
        {
            await database.CreateCollectionAsync("users");
            await database.CreateCollectionAsync("comments");
        }
    }
}
