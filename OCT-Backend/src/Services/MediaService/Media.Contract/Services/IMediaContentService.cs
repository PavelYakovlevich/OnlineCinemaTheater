using Media.Domain;

namespace Media.Contract.Services;

public interface IMediaContentService
{
    Task<Guid> CreateMediaContentAsync(MediaContentModel mediaContentModel);

    IAsyncEnumerable<MediaContentModel> GetMediaContentAsync(Guid mediaId);

    Task<Stream> GetMediaContentFileAsync(Guid mediaId);

    Task<Stream> GetMediaContentFileAsync(Guid mediaId, int seriaNumber);

    Task DeleteMediaContentAsync(Guid id);

    Task DeleteMediaContentAsync(Guid id, int seriaNumber);
}