using AutoMapper;
using Comment.Domain;
using Messages.UserService;
using Models.CommentService.Requests;
using Models.CommentService.Responses;

namespace Comment.API.Extensions;

internal static class MapperExtensions
{
    public static void AddApiToModelsMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<CreateCommentRequest, CommentModel>();
        config.CreateMap<CommentModel, GetCommentResponse>();
        config.CreateMap<GetCommentsFilters, CommentsFilters>();
        config.CreateMap<UpdateCommentRequest, CommentModel>();
        config.CreateMap<UserModel, GetUserResponse>();
    }

    public static void AddMessagesToModelsMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<UserUpdatedMessage, UserModel>()
            .ForMember(_ => _.Id, c => c.MapFrom(_ => _.UserId));
    }
}