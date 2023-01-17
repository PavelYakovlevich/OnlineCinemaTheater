using BlobStorage.Abstractions;
using Configurations.BlobStorage;
using Exceptions;
using Media.Contract.Repositories;
using Media.Contract.Services;
using Media.Domain;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Media.Core;

public class MediaContentService : IMediaContentService
{
    private readonly BlobsServiceBase<IFormFile> _blobService;
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaInfoRepository _mediaInfoRepository;

    public MediaContentService(BlobsServiceBase<IFormFile> blobService, IMediaRepository mediaRepository, IMediaInfoRepository mediaInfoRepository)
    {
        _blobService = blobService;
        _mediaRepository = mediaRepository;
        _mediaInfoRepository = mediaInfoRepository;
    }

    public async Task<Guid> CreateMediaContentAsync(MediaContentModel mediaContentModel)
    {
        if (await MediaContentExistsAsync(mediaContentModel))
        {
            throw new BadRequestException($"Media content for media with id {mediaContentModel.MediaId} exists");
        }

        mediaContentModel.Id = Guid.NewGuid();
        mediaContentModel.IssueDate = DateTime.UtcNow;
        mediaContentModel.Number--;

        var id = await _mediaRepository.CreateAsync(mediaContentModel);
        Log.Debug("Media content was created: {@media}", mediaContentModel);

        var link = await _blobService.UploadBlobAsync(BlobContainersNames.MediasContent, mediaContentModel.ContentFile, id);
        Log.Debug("Media content file was uploaded: {link}", link);

        return id;
    }

    public async IAsyncEnumerable<MediaContentModel> GetMediaContentAsync(Guid mediaId)
    {
        var mediasModels = _mediaRepository.ReadAllByMediaIdAsync(mediaId) ??
            throw new ResourceNotFoundException($"Media with id {mediaId} was not found");

        int counter = 0;
        await foreach (var media in mediasModels)
        {
            yield return media;
            counter++;
        }

        Log.Debug("{count} media content items were found", counter);
    }

    public async Task DeleteMediaContentAsync(Guid id) =>
        await DeleteMediaContentCoreAsync(id);

    public async Task DeleteMediaContentAsync(Guid id, int seriaNumber) =>
        await DeleteMediaContentCoreAsync(id, seriaNumber - 1);

    public async Task<Stream> GetMediaContentFileAsync(Guid mediaId) =>
        await GetMediaContentFileCoreAsync(mediaId);

    public async Task<Stream> GetMediaContentFileAsync(Guid mediaId, int seriaNumber) =>
        await GetMediaContentFileCoreAsync(mediaId, seriaNumber - 1);

    private async Task<bool> MediaContentExistsAsync(MediaContentModel mediaContentModel)
    {
        var mediaInfo = await _mediaInfoRepository.ReadMediaInfoAsync(mediaContentModel.MediaId) ??
            throw new ResourceNotFoundException($"Media with id {mediaContentModel.MediaId} was not found");

        var mediaContent = _mediaRepository.ReadAllByMediaIdAsync(mediaContentModel.MediaId);
        if (!mediaInfo.IsTvSerias)
        {
            return await mediaContent.AnyAsync();
        }

        return await mediaContent.AnyAsync(seria => seria.Number == mediaContentModel.Number - 1);
    }

    private async Task<Stream> GetMediaContentFileCoreAsync(Guid mediaId, int? seriaNumber = null)
    {
        var mediaContentId = await GetMediaContentId(mediaId, seriaNumber);

        var blobStream = await _blobService.GetBlobAsync(BlobContainersNames.MediasContent, mediaContentId);
        Log.Debug("Blob for media content with id {id} was found", mediaId);

        return blobStream;
    }

    private async Task DeleteMediaContentCoreAsync(Guid mediaId, int? seriaNumber = null)
    {
        var mediaContentId = await GetMediaContentId(mediaId, seriaNumber);

        await _mediaRepository.DeleteByIdAsync(mediaContentId);

        Log.Debug("Media content {contentId} with for media {mediaId} was deleted", mediaContentId, mediaId);

        await _blobService.DeleteBlobAsync(BlobContainersNames.MediasContent, mediaContentId);

        Log.Debug("Media blob {blobId} for media {mediaId} was deleted", mediaContentId, mediaId);
    }

    private async Task<Guid> GetMediaContentId(Guid mediaId, int? seriaNumber = null)
    {
        _ = await _mediaInfoRepository.ReadMediaInfoAsync(mediaId) ??
            throw new ResourceNotFoundException($"Media {mediaId} was not found");

        var mediaContentModel = await _mediaRepository
            .ReadAllByMediaIdAsync(mediaId)
            .ElementAtOrDefaultAsync(seriaNumber ?? 0);

        return mediaContentModel?.Id ?? 
            throw new ResourceNotFoundException($"Media content for media {mediaId}({seriaNumber ?? 0}) was not found");
    }
}
