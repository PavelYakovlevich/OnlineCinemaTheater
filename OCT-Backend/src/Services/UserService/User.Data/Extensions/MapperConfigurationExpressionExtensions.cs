using AutoMapper;
using User.Domain.Models;
using UserEntity = User.Data.Entities.User;

namespace User.Data.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static void AddModelsToEntitiesMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<UserModel, UserEntity>()
            .ForMember(u => u.Photo, configure => configure.MapFrom(um => um.PhotoLink))
            .ReverseMap();
    }
}
