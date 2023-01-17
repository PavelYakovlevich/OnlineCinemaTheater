using Authentication.Domain.Models;
using AutoMapper;
using Models.AuthenticationService.Request;
using Models.AuthenticationService.Response;

namespace Authentication.API.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static void ConfigApiToServicesModelsMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<AccountRequest, AccountModel>()
            .ReverseMap();

        config.CreateMap<ChangeAccountRequest, AccountModel>();

        config.CreateMap<AccountModel, AccountResponse>();
    }
}
