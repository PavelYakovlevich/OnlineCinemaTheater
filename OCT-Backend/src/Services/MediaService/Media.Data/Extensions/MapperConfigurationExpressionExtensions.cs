using AutoMapper;
using Media.Data.Entities;
using Media.Domain;

namespace Media.Data.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static void AddModelsToEntitesMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<MediaInfoModel, MediaInfo>().ReverseMap();
        config.CreateMap<MediaContentModel, MediaContent>().ReverseMap();
    }
}
