using Authentication.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Extensions;

public static class WebApplicationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        AuthenticationServiceDbContext context = app.Services
            .CreateScope().ServiceProvider.GetRequiredService<AuthenticationServiceDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
}
