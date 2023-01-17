using AutoMapper;
using MediaInfo.Domain.Genre;
using MediaInfo.Domain.MediaInfo;
using MediaInfo.Domain.Participant;

namespace MediaInfo.Domain.Extensions;

public static class MapperExtensions
{
    public static void AddCoreModelsMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<MediaInfoModel, PartialUpdateMediaInfoModel>();

        config.CreateMap<IEnumerable<ParticipantModel>, IEnumerable<Guid>>()
            .ConstructUsing(participants => participants.Select(p => p.Id).ToList());

        config.CreateMap<IEnumerable<GenreModel>, IEnumerable<Guid>>()
            .ConstructUsing(genres => genres.Select(g => g.Id).ToList());
    }
}
