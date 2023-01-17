using Comment.Contract.Repositories;
using Comment.Contract.Services;
using Comment.Domain;
using Exceptions;
using Serilog;

namespace Comment.Core;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateAsync(Guid id)
    {
        await _repository.CreateAsync(id);

        Log.Debug("User {id} was created", id);

        return id;
    }

    public async Task UpdateAsync(Guid id, UserModel userModel)
    {
        if (!await _repository.UpdateAsync(id, userModel))
        {
            throw new ResourceNotFoundException($"User {id} was not found");
        }

        Log.Debug("User {id} was updated: {@model}", id, userModel);
    }
}
