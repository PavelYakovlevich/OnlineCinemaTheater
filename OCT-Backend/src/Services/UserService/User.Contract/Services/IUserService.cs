using Microsoft.AspNetCore.Http;
using User.Domain.Models;

namespace User.Contract.Services;

public interface IUserService
{
    Task CreateUserAsync(Guid accountId);

    Task<UserModel> UpdateUserAsync(Guid id, UserModel userModel);

    Task<UserModel> GetUserByIdAsync(Guid id);

    Task UploadUserPhoto(Guid id, IFormFile photo);

    Task DeleteUserPhotoAsync(Guid id);

    Task<Stream> GetUserPhotoAsync(Guid id);
}
