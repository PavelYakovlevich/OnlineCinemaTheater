using MediaInfo.Domain.MediaInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace MediaInfo.Contract.Services;

public interface IMediaInfoService
{
    Task<Guid> CreateAsync(MediaInfoModel model);

    Task<MediaInfoModel> GetByIdAsync(Guid id);

    IAsyncEnumerable<MediaInfoModel> GetAllAsync(MediaInfoFiltersModel filters);

    Task UpdateAsync(Guid id, PartialUpdateMediaInfoModel model);

    Task DeleteAsync(Guid id);

    Task UploadPictureAsync(Guid id, IFormFile picture);

    Task DeletePictureAsync(Guid id);

    Task<Stream> GetPictureAsync(Guid id);
}
