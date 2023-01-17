using AutoMapper;
using Models.UserService.Requests;
using Models.UserService.Responses;
using User.Domain.Models;

namespace User.API.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static void AddApiToModelsMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<ChangeUserRequest, UserModel>();

        config.CreateMap<UserModel, GetUserReponse>();
    }
}

