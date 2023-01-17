using Media.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Media.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        MediaServiceDbContext context = app.Services
            .CreateScope().ServiceProvider.GetRequiredService<MediaServiceDbContext>();

        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            await context.Database.MigrateAsync();
        }
    }
}
