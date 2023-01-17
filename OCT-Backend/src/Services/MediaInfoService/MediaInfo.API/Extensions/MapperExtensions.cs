using AutoMapper;
using Enums.MediaInfoService;
using MediaInfo.Domain.Genre;
using MediaInfo.Domain.MediaInfo;
using MediaInfo.Domain.Participant;
using Models.MediaInfoService.Request.Genre;
using Models.MediaInfoService.Request.MediaInfo;
using Models.MediaInfoService.Request.Participant;
using Models.MediaInfoService.Response.Genre;
using Models.MediaInfoService.Response.MediaInfo;
using Models.MediaInfoService.Response.Participant;

namespace MediaInfo.API.Extensions;

internal static class MapperExtensions
{
    public static void AddApiToModelsMapping(this IMapperConfigurationExpression config)
    {
        config.CreateMap<ParticipantRequest, ParticipantModel>();
        config.CreateMap<ParticipantsRequestFilters, ParticipantFiltersModel>();
        config.CreateMap<ParticipantRole, string>().ConvertUsing(e => e.ToString());
        config.CreateMap<ParticipantModel, ParticipantResponse>();

        config.CreateMap<GenreRequest, GenreModel>();
        config.CreateMap<GenresRequestFilters, GenreFiltersModel>();
        config.CreateMap<GenreModel, GenreResponse>();

        config.CreateMap<IEnumerable<Guid>, IEnumerable<ParticipantModel>>()
            .ConstructUsing(ids => ids.Select(id => new ParticipantModel { Id = id }).ToList())
            .ReverseMap();
        config.CreateMap<IEnumerable<Guid>, IEnumerable<GenreModel>>()
            .ConstructUsing(ids => ids.Select(id => new GenreModel { Id = id }).ToList())
            .ReverseMap();

        config.CreateMap<MediaInfoRequest, MediaInfoModel>();
        config.CreateMap<MediaInfoModel, GetMediaResponse>();
        config.CreateMap<PartialUpdateMediaInfoRequest, PartialUpdateMediaInfoModel>();
        config.CreateMap<MediaInfoRequestFilters, MediaInfoFiltersModel>();
    }

    public static async IAsyncEnumerable<TResult> Map<TSource, TResult>(this IMapper mapper, IAsyncEnumerable<TSource> source)
    {
        await foreach(var sourceItem in source)
        {
            yield return mapper.Map<TSource, TResult>(sourceItem);
        }
    }

    private class GuidParticipantResolver : IValueResolver<MediaInfoRequest, MediaInfoModel, IList<ParticipantModel>>
    {
        public IList<ParticipantModel> Resolve(MediaInfoRequest source, MediaInfoModel destination, IList<ParticipantModel> destMember, ResolutionContext context)
        {
            return source.Participants.Select(id => new ParticipantModel
            {
                Id = id
            }).ToList();
        }
    }

    private class GuidGenresResolver : IValueResolver<MediaInfoRequest, MediaInfoModel, IList<GenreModel>>
    {
        public IList<GenreModel> Resolve(MediaInfoRequest source, MediaInfoModel destination, IList<GenreModel> destMember, ResolutionContext context)
        {
            return source.Participants.Select(id => new GenreModel
            {
                Id = id
            }).ToList();
        }
    }
}
