using Microsoft.AspNetCore.Http;

namespace Media.Contract.Services;

public interface ITrailerService
{

    Task<Stream> GetTrailerAsync(Guid mediaId);

    Task DeleteTrailerAsync(Guid mediaId);

    Task UploadTrailerAsync(Guid mediaId, IFormFile mediaFile);
}
