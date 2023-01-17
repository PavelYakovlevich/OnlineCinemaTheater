using Comment.Domain;

namespace Comment.Contract.Services;

public interface IUserService
{
    Task<Guid> CreateAsync(Guid id);

    Task UpdateAsync(Guid id, UserModel userModel);
}
