using Authentication.Core.Configuration;
using Authentication.Domain.Models;
using Bogus;
using Configurations.Authentication;
using Enums.AuthentificationService;
using Models.AuthenticationService.Request;

namespace AuthenticationService.Tests;

internal static class Fakers
{
    public static Faker<JWTConfiguration> JWTConfigurationFaker = new Faker<JWTConfiguration>()
        .RuleFor(p => p.Key, f => f.Random.Hash())
        .RuleFor(p => p.Issuer, f => f.Random.Word())
        .RuleFor(p => p.ExpiresInMinutes, f => f.Random.Int(1, 100));

    public static Faker<AccountModel> AccountFaker = new Faker<AccountModel>()
        .RuleFor(p => p.Id, f => f.Random.Guid())
        .RuleFor(p => p.Password, f => f.Internet.Password())
        .RuleFor(p => p.EmailConfirmed, f => false)
        .RuleFor(p => p.Email, f => f.Internet.Email())
        .RuleFor(p => p.RegistrationDate, f => DateTime.UtcNow.AddMinutes(f.Random.UInt(1, 100)))
        .RuleFor(p => p.Role, f => f.Random.Enum<AccountRole>());

    public static Faker<RefreshTokenModel> TokenFaker = new Faker<RefreshTokenModel>()
        .RuleFor(p => p.Id, f => f.Random.Guid())
        .RuleFor(p => p.Token, f => f.Random.Hash())
        .RuleFor(p => p.IssueDate, f => DateTime.UtcNow)
        .RuleFor(p => p.AccountId, f => f.Random.Guid())
        .RuleFor(p => p.Account, f => AccountFaker.Generate());

    public static Faker<ApplicationConfiguration> ApplicationConfigurationFaker = new Faker<ApplicationConfiguration>()
        .RuleFor(p => p.Salt, f => BCrypt.Net.BCrypt.GenerateSalt())
        .RuleFor(p => p.JwtConfiguration, f => JWTConfigurationFaker.Generate())
        .RuleFor(p => p.RefreshTokenExpirationTimeInMinutes, f => f.Random.Int(1, 100))
        .RuleFor(p => p.TokenLength, f => f.Random.Int(1, 1000));

    public static Faker<AccountRequest> AccountRequestFaker = new Faker<AccountRequest>()
        .RuleFor(p => p.Password, f => f.Internet.Password())
        .RuleFor(p => p.Email, f => f.Internet.Email());
}
