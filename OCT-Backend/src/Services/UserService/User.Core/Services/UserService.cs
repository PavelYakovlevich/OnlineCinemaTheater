using BlobStorage.Abstractions;
using Configurations.BlobStorage;
using Exceptions.UserService;
using Infrastructure.MessageBroker.Abstractions;
using Microsoft.AspNetCore.Http;
using Serilog;
using User.Contract.Repositories;
using User.Contract.Services;
using User.Domain.Models;

namespace User.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly BlobsServiceBase<IFormFile> _imageService;
    private readonly IUserPublishService _userPublishService;

    public UserService(IUserRepository userRepository, BlobsServiceBase<IFormFile> imageService, IUserPublishService userPublishService)
    {
        _userRepository = userRepository;
        _imageService = imageService;
        _userPublishService = userPublishService;
    }

    public async Task CreateUserAsync(Guid accountId)
    {
        await _userRepository.CreateUserAsync(new UserModel
        {
            Id = accountId,
        });

        await _userPublishService.PublishUserCreatedMessage(accountId);

        Log.Debug("User with id {id} was created", accountId);
    }

    public async Task<UserModel> GetUserByIdAsync(Guid id)
    {
        var userModel = await _userRepository.GetUserById(id) ?? 
            throw new UserOperationException($"User with id {id} doesn't exist");

        Log.Debug("User with id {id} was found", id);

        return userModel;
    }

    public async Task<UserModel> UpdateUserAsync(Guid id, UserModel userModel)
    {
        if(!await _userRepository.UpdateUserAsync(id, userModel))
        {
            throw new UserOperationException($"User with id {id} doesn't exist");
        }

        Log.Debug("User with id {id} was updated", id);

        await _userPublishService.PublishUserUpdatedMessage(id, userModel.Name, userModel.Surname);

        userModel.Id = id;
        return userModel;
    }

    public async Task UploadUserPhoto(Guid id, IFormFile photo)
    {
        var userModel = await GetUserByIdAsync(id) ??
            throw new UserOperationException($"User with id {id} doesn't exist");

        var photoLink = await _imageService.UploadBlobAsync(BlobContainersNames.UsersProfilePictures, photo, id);
        userModel.PhotoLink = photoLink;

        await UpdateUserAsync(id, userModel);
        Log.Debug("Photo of user with id {id} was updated", id);
    }

    public async Task DeleteUserPhotoAsync(Guid id)
    {
        var userModel = await _userRepository.GetUserById(id) ??
             throw new UserOperationException($"User with id {id} doesn't exist");

        userModel.PhotoLink = null;

        await _imageService.DeleteBlobAsync(BlobContainersNames.UsersProfilePictures, id);
        Log.Debug("Photo of user with id {id} was deleted from storage", id);

        await _userRepository.UpdateUserAsync(id, userModel);
        Log.Debug("Photo link of user with id {id} was deleted", id);
    }

    public async Task<Stream> GetUserPhotoAsync(Guid id)
    {
        _ = await _userRepository.GetUserById(id) ??
            throw new UserOperationException($"User with id {id} doesn't exist");

        return await _imageService.GetBlobAsync(BlobContainersNames.UsersProfilePictures, id) ??
            throw new BlobOperationException($"Image for user with id {id} doesn't exist");
    }
}
