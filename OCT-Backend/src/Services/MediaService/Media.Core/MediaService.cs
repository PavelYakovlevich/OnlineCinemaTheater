using BlobStorage.Abstractions;
using Configurations.BlobStorage;
using Exceptions;
using Media.Contract.Repositories;
using Media.Contract.Services;
using Media.Domain;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Media.Core;

public class MediaService : IMediaService
{
    private readonly BlobsServiceBase<IFormFile> _blobService;
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaInfoRepository _mediaInfoRepository;

    public MediaService(BlobsServiceBase<IFormFile> blobService, IMediaRepository mediaRepository, IMediaInfoRepository mediaInfoRepository)
    {
        _blobService = blobService;
        _mediaRepository = mediaRepository;
        _mediaInfoRepository = mediaInfoRepository;
    }

    public async Task<Guid> CreateMediaAsync(MediaInfoModel mediaInfoModel)
    {
        var id = await _mediaInfoRepository.CreateMediaInfoAsync(mediaInfoModel);

        Log.Debug("Media information with id {id} was created", id);

        return id;
    }

    public async Task DeleteMediaAsync(Guid id)
    {
        var medias = _mediaRepository.ReadAllByMediaIdAsync(id) ??
            throw new ResourceNotFoundException($"Media information with id {id} was not found");

        await DeleteAllMediaBlobs(medias);

        await _mediaInfoRepository.DeleteMediaInfoAsync(id);

        Log.Debug("Media information with id {id} was deleted", id);
    }

    public async Task UpdateMediaAsync(Guid id, MediaInfoModel mediaInfoModel)
    {
        if (!await _mediaInfoRepository.UpdateMediaInfoAsync(id, mediaInfoModel))
        {
            throw new ResourceNotFoundException($"Media information with id {id} was not found");
        }

        Log.Debug("Media information with id {id} was updated: {@mediaInfo}", id, mediaInfoModel);
    }

    private async Task DeleteAllMediaBlobs(IAsyncEnumerable<MediaContentModel> medias)
    {
        await foreach (var media in medias)
        {
            await _blobService.DeleteBlobAsync(BlobContainersNames.MediasContent, media.Id);
            Log.Debug("Media blob with id {id} was deleted", media.Id);
        }
    }
}
