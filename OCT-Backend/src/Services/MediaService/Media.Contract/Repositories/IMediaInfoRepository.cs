using Media.Domain;

namespace Media.Contract.Repositories;

public interface IMediaInfoRepository
{
    Task<Guid> CreateMediaInfoAsync(MediaInfoModel mediaInfoModel, CancellationToken token = default);

    Task<bool> DeleteMediaInfoAsync(Guid id, CancellationToken token = default);

    Task<bool> UpdateMediaInfoAsync(Guid id, MediaInfoModel mediaInfoModel, CancellationToken token = default);

    Task<MediaInfoModel> ReadMediaInfoAsync(Guid id, CancellationToken token = default);
}
