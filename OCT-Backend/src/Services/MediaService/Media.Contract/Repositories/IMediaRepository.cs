using Media.Domain;

namespace Media.Contract.Repositories;

public interface IMediaRepository
{
    Task<MediaContentModel> ReadByIdAsync(Guid id, CancellationToken token = default);

    IAsyncEnumerable<MediaContentModel> ReadAllByMediaIdAsync(Guid mediaId, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<Guid> CreateAsync(MediaContentModel mediaInfoModel, CancellationToken token = default);
}
