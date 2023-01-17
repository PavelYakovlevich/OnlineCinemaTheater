using BlobStorage.Abstractions;
using Configurations.BlobStorage;
using Exceptions;
using Exceptions.UserService;
using MediaInfo.Contract.Repositories;
using MediaInfo.Contract.Services;
using MediaInfo.Domain.Participant;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace MediaInfo.Core.Services;

public class ParticipantService : IParticipantService
{
    private readonly IParticipantRepository _repository;
    private readonly BlobsServiceBase<IFormFile> _blobsService;

    public ParticipantService(IParticipantRepository repository, BlobsServiceBase<IFormFile> blobsService)
    {
        _repository = repository;
        _blobsService = blobsService;
    }

    public async Task<Guid> CreateAsync(ParticipantModel model)
    {
        var id = await _repository.CreateAsync(model);

        Log.Debug("New participant '{id}' was created: {@model}", id, model);

        return id;
    }

    public async Task DeleteAsync(Guid id)
    {
        await this.DeletePictureAsync(id);

        var participantWasDeleted = await _repository.DeleteAsync(id);

        if (!participantWasDeleted)
        {
            throw new ResourceNotFoundException($"Participant with id {id} doesn't exist");
        }

        Log.Debug("Participant with id {id} was deleted", id);
    }

    public async Task DeletePictureAsync(Guid id)
    {
        var participant = await _repository.ReadById(id) ??
            throw new ResourceNotFoundException($"Participant with id {id} doesn't exist");

        if (participant.Picture is not null)
        {
            await _blobsService.DeleteBlobAsync(BlobContainersNames.ParticipantsProfilePictures, id);
            Log.Debug("Photo of user with id {id} was deleted from storage", id);
        }
        
        participant.Picture = null;

        await _repository.UpdateAsync(id, participant);
        Log.Debug("Photo link of participant with id {id} was deleted", id);
    }

    public async IAsyncEnumerable<ParticipantModel> FilterAsync(ParticipantFiltersModel filtersModel)
    {
        var participants = _repository.FindAllAsync(filtersModel);

        Log.Debug("{count} participants were found with filters {@filter}", await participants.CountAsync(), filtersModel);

        await foreach (var participant in participants)
        {
            yield return participant;
        }
    }

    public async Task<ParticipantModel> GetByIdAsync(Guid id)
    {
        var participant = await _repository.ReadById(id) ??
            throw new ResourceNotFoundException($"Participant with id {id} doesn't exist");

        Log.Debug("Participant with id {id} was found: {@model}", id, participant);

        return participant;
    }

    public async Task<Stream> GetPictureAsync(Guid id)
    {
        _ = await _repository.ReadById(id) ??
            throw new ResourceNotFoundException($"Participant with id {id} doesn't exist");

        return await _blobsService.GetBlobAsync(BlobContainersNames.ParticipantsProfilePictures, id) ??
            throw new BlobOperationException($"Image for participant with id {id} doesn't exist");
    }

    public async Task UpdateAsync(Guid id, ParticipantModel model)
    {
        if (!await _repository.UpdateAsync(id, model))
        {
            throw new ResourceNotFoundException($"Participant with id {id} doesn't exist");
        }

        Log.Debug("Participant with id {id} was updated: {@model}", id, model);
    }

    public async Task UploadPictureAsync(Guid id, IFormFile file)
    {
        var participant = await _repository.ReadById(id) ??
            throw new ResourceNotFoundException($"Participant with id {id} doesn't exist");

        participant.Picture = await _blobsService.UploadBlobAsync(BlobContainersNames.ParticipantsProfilePictures, file, id);
        Log.Debug("Photo of participant with id {id} was uploaded to storage", id);

        await _repository.UpdateAsync(id, participant);
        Log.Debug("Photo link of participant with id {id} was deleted", id);
    }
}
