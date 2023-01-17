using MediaInfo.Domain.MediaInfo;

namespace MediaInfo.Contract.Repositories;

public interface IMediaInfoRepository
{
    Task<Guid> CreateAsync(MediaInfoModel model);

    Task<MediaInfoModel> ReadByIdAsync(Guid id);

    IAsyncEnumerable<MediaInfoModel> ReadAllAsync(MediaInfoFiltersModel filters);

    Task<bool> UpdateAsync(Guid id, PartialUpdateMediaInfoModel model);

    Task<bool> DeleteAsync(Guid id);
}
