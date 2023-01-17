using Comment.Domain;

namespace Comment.Contract.Repositories;

public interface IUserRepository
{
    Task CreateAsync(Guid id, CancellationToken token = default);

    Task<UserModel> ReadByIdAsync(Guid id, CancellationToken token = default);

    Task<bool> UpdateAsync(Guid id, UserModel userModel, CancellationToken token = default);
}
