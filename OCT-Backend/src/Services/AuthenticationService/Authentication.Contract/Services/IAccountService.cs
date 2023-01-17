using Authentication.Domain.Models;
using Enums.AuthentificationService;

namespace Authentication.Contract.Services;

public interface IAccountService
{
    Task<AccountModel> CreateAccountAsync(AccountModel accountModel, AccountRole accountRole);

    Task<AccountModel> GetAccountByIdAsync(Guid id);

    Task<AccountModel> GetAccountByCredentialsAsync(string email, string password);

    Task ConfirmAccountEmailAsync(string token);

    Task SendChangePasswordEmailAsync(string email);

    Task UpdatePasswordAsync(Guid id, string token, string password);
}