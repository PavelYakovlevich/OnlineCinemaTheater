using User.Domain.Models;

namespace User.Contract.Repositories;

public interface IUserRepository
{
    Task CreateUserAsync(UserModel userModel);

    Task<UserModel> GetUserById(Guid id);

    Task<bool> UpdateUserAsync(Guid id, UserModel userModel);
}
