using Authentication.Domain.Models;

namespace Authentication.Contract.Repositories;

public interface IEmailConfirmationTokenRepository
{
    Task<EmailConfirmationTokenModel> ReadByTokenValueAsync(string token);

    Task<EmailConfirmationTokenModel> CreateAsync(EmailConfirmationTokenModel tokenModel);
}