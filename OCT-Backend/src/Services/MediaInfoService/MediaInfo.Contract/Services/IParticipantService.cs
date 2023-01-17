using MediaInfo.Domain.Participant;
using Microsoft.AspNetCore.Http;

namespace MediaInfo.Contract.Services;

public interface IParticipantService
{
    Task<Guid> CreateAsync(ParticipantModel model);

    Task<ParticipantModel> GetByIdAsync(Guid id);

    IAsyncEnumerable<ParticipantModel> FilterAsync(ParticipantFiltersModel filtersModel);

    Task UpdateAsync(Guid id, ParticipantModel model);

    Task DeleteAsync(Guid id);

    Task UploadPictureAsync(Guid id, IFormFile file);

    Task DeletePictureAsync(Guid id);

    Task<Stream> GetPictureAsync(Guid id);
}
