using Media.Domain;

namespace Media.Contract.Services;

public interface IMediaService
{
    Task<Guid> CreateMediaAsync(MediaInfoModel mediaInfoModel);

    Task UpdateMediaAsync(Guid mediaId, MediaInfoModel mediaInfoModel);

    Task DeleteMediaAsync(Guid mediaId);
}
