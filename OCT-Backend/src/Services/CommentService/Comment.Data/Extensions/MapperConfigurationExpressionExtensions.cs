using AutoMapper;
using Comment.Data.Entities;
using Comment.Domain;
using CommentEntity = Comment.Data.Entities.Comment;

namespace Comment.Data.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static void AddModelsToEntitesMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<UserModel, User>()
            .ReverseMap();

        config.CreateMap<CommentModel, CommentEntity>()
            .ReverseMap();
    }
}
