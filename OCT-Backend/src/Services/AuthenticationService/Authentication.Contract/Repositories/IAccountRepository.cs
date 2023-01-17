using Authentication.Domain.Models;

namespace Authentication.Contract.Repositories;

public interface IAccountRepository
{
    Task<AccountModel> CreateAsync(AccountModel account);

    Task<AccountModel> ReadByIdAsync(Guid id);

    Task<AccountModel> ReadByEmailAsync(string email);

    Task<AccountModel> ReadByCredentialsAsync(string email, string password);

    Task<bool> UpdateAsync(Guid id, AccountModel account);
    
}
