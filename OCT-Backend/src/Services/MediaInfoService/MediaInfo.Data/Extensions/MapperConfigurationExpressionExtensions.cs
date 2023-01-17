using AutoMapper;
using MediaInfo.Data.Entities;
using MediaInfo.Domain.Genre;
using MediaInfo.Domain.MediaInfo;
using MediaInfo.Domain.Participant;

namespace MediaInfo.Data.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static void AddModelsToEntitiesMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<ParticipantModel, Participant>()
            .ReverseMap();

        config.CreateMap<GenreModel, Genre>()
            .ReverseMap();

        config.CreateMap<MediaInfoModel, MediaInformation>()
            .ReverseMap();
    }
}
