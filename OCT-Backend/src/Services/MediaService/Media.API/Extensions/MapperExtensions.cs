using AutoMapper;
using Media.Domain;
using Messages.MediaInfoService;
using Models.MediaService.Responses;

namespace Media.API.Extensions;

internal static class MapperExtensions
{
    public static void AddApiToModelsMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<MediaInfoCreatedMessage, MediaInfoModel>();
        config.CreateMap<MediaInfoUpdatedMessage, MediaInfoModel>();

        config.CreateMap<MediaContentModel, MediaContentResponse>();
    }
}
