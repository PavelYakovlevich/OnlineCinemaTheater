using Authentication.Data.Context.EntityConfigurations;
using Authentication.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Context;

public class AuthenticationServiceDbContext : DbContext
{
    public AuthenticationServiceDbContext(DbContextOptions<AuthenticationServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new EmailConfirmationTokenConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}
