using BlobStorage.Abstractions;
using Configurations.BlobStorage;
using Exceptions;
using Media.Contract.Repositories;
using Media.Contract.Services;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Media.Core;

public class TrailerService : ITrailerService
{
    private readonly BlobsServiceBase<IFormFile> _blobService;
    private readonly IMediaInfoRepository _repository;

    public TrailerService(BlobsServiceBase<IFormFile> blobService, IMediaInfoRepository repository)
    {
        _blobService = blobService;
        _repository = repository;
    }

    public async Task DeleteTrailerAsync(Guid mediaId)
    {
        _ = await _repository.ReadMediaInfoAsync(mediaId) ??
            throw new ResourceNotFoundException($"Trailer for media {mediaId} was not found");

        await _blobService.DeleteBlobAsync(BlobContainersNames.Trailers, mediaId);

        Log.Debug("Trailer for media {id} was deleted", mediaId);
    }

    public async Task<Stream> GetTrailerAsync(Guid mediaId)
    {
        var trailerStream = await _blobService.GetBlobAsync(BlobContainersNames.Trailers, mediaId) ??
            throw new ResourceNotFoundException($"Trailer for media {mediaId} was not found");

        Log.Debug("Trailer for media {id} was found", mediaId);

        return trailerStream;
    }

    public async Task UploadTrailerAsync(Guid mediaId, IFormFile mediaFile)
    {
        _ = await _repository.ReadMediaInfoAsync(mediaId) ?? 
            throw new ResourceNotFoundException($"Media {mediaId} was not found");

        var link = await _blobService.UploadBlobAsync(BlobContainersNames.Trailers, mediaFile, mediaId);

        Log.Debug("Trailer for media {id} was uploaded: {link}", mediaId, link);
    }
}
