using Authentication.Data.Entities;
using Authentication.Domain.Models;
using AutoMapper;

namespace Authentication.Data.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static void ConfigModelsToEntitesMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<AccountModel, Account>();
        config.CreateMap<Account, AccountModel>();

        config.CreateMap<EmailConfirmationTokenModel, EmailConfirmationToken>();
        config.CreateMap<EmailConfirmationToken, EmailConfirmationTokenModel>();

        config.CreateMap<RefreshTokenModel, RefreshToken>();
        config.CreateMap<RefreshToken, RefreshTokenModel>();
    }
}
