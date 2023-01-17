using MediaInfo.Domain.Participant;

namespace MediaInfo.Contract.Repositories;

public interface IParticipantRepository
{
    Task<Guid> CreateAsync(ParticipantModel model);

    Task<bool> UpdateAsync(Guid id, ParticipantModel model);

    Task<bool> DeleteAsync(Guid id);

    Task<ParticipantModel> ReadById(Guid id);

    IAsyncEnumerable<ParticipantModel> FindAllAsync(ParticipantFiltersModel filtersModel);
}
