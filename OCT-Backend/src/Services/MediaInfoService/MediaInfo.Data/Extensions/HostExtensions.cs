using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MediaInfo.Data.Extensions;

public static class HostExtensions
{
    public static async Task<IHost> ApplyMigrations(this IHost host, string dbName)
    {
        using var scope = host.Services.CreateScope();

        var databaseService = scope.ServiceProvider.GetRequiredService<Database>();

        await databaseService.EnsureCreatedAsync(dbName);

        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        runner.ListMigrations();
        runner.MigrateUp();

        return host;
    }
}
