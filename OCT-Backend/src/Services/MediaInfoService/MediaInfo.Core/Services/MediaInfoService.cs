using AutoMapper;
using BlobStorage.Abstractions;
using Configurations.BlobStorage;
using Exceptions;
using Infrastructure.MessageBroker.Abstractions;
using MediaInfo.Contract.Repositories;
using MediaInfo.Contract.Services;
using MediaInfo.Domain.MediaInfo;
using Messages.MediaInfoService;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace MediaInfo.Core.Services;

public class MediaInfoService : IMediaInfoService
{
    private readonly IMediaInfoRepository _mediaRepository;
    private readonly BlobsServiceBase<IFormFile> _blobService;
    private readonly IMediaInfoPublishService _publishService;
    private readonly IMapper _mapper;

    public MediaInfoService(IMediaInfoRepository mediaRepository,
        BlobsServiceBase<IFormFile> blobService,
        IMediaInfoPublishService publishService,
        IMapper mapper)
    {
        _mediaRepository = mediaRepository;
        _blobService = blobService;
        _publishService = publishService;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(MediaInfoModel model)
    {
        var id = await _mediaRepository.CreateAsync(model);
        Log.Debug("Media with id {id} was created: {@media}", id, model);

        await _publishService.PublishMediaInfoCreatedMessage(id, model.IsTvSerias, model.IsVisible);
        Log.Debug("Message {message} was published", nameof(MediaInfoCreatedMessage));

        return id;
    }

    public async Task DeleteAsync(Guid id)
    {
        if(!await _mediaRepository.DeleteAsync(id))
        {
            throw new ResourceNotFoundException($"Media info with id {id} was not found");
        }

        Log.Debug("Media with id {id} was deleted", id);

        await DeletePictureCoreAsync(id, false);

        await _publishService.PublishMediaInfoDeletedMessage(id);
        Log.Debug("Message {message} was published", nameof(MediaInfoDeletedMessage));
    }

    public async Task DeletePictureAsync(Guid id) => await this.DeletePictureCoreAsync(id);

    public async IAsyncEnumerable<MediaInfoModel> GetAllAsync(MediaInfoFiltersModel filters)
    {
        filters.Genres = filters.Genres?.Select(g => g.ToUpper());

        var medias = _mediaRepository.ReadAllAsync(filters);

        Log.Debug("{count} medias were found with filters {@filter}", await medias.CountAsync(), filters);

        await foreach (var media in medias)
        {
            yield return media;
        }
    }

    public async Task<MediaInfoModel> GetByIdAsync(Guid id)
    {
        var mediaInfoModel = await _mediaRepository.ReadByIdAsync(id) ??
            throw new ResourceNotFoundException($"Media info with id {id} was not found");

        Log.Debug("Media with id {id} was found: {@media}", id, mediaInfoModel);

        return mediaInfoModel;
    }

    public async Task<Stream> GetPictureAsync(Guid id)
    {
        var stream = await _blobService.GetBlobAsync(BlobContainersNames.MediaInfoPictures, id) ??
            throw new ResourceNotFoundException($"Media info with id {id} was not found");

        Log.Debug("Picture for media with id {id} was found", id);

        return stream;
    }

    public async Task UpdateAsync(Guid id, PartialUpdateMediaInfoModel model)
    {
        if (!await _mediaRepository.UpdateAsync(id, model))
        {
            throw new ResourceNotFoundException($"Media info with id {id} was not found");
        }

        Log.Debug("Media {id} was partially updated: {@model}", id, model);

        await _publishService.PublishMediaInfoUpdatedMessage(id,
            model.IsTvSerias,
            model.IsVisible);

        Log.Debug("Message {message} was published", nameof(MediaInfoUpdatedMessage));
    }

    public async Task UploadPictureAsync(Guid id, IFormFile picture)
    {
        _ = await _mediaRepository.ReadByIdAsync(id) ??
            throw new ResourceNotFoundException($"Media info with id {id} was not found");

        var link = await _blobService.UploadBlobAsync(BlobContainersNames.MediaInfoPictures, picture, id);

        Log.Debug("Picture for media with id {id} was uploaded: {link}", id, link);
    }

    private async Task DeletePictureCoreAsync(Guid id, bool checkMediaExistence = true)
    {
        if (checkMediaExistence)
        {
            _ = await _mediaRepository.ReadByIdAsync(id) ??
                throw new ResourceNotFoundException($"Media info with id {id} was not found");
        }

        await _blobService.DeleteBlobAsync(BlobContainersNames.MediaInfoPictures, id);
    }
}
