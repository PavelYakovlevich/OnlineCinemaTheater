using Authentication.Domain.Models;

namespace Authentication.Contract.Services;

public interface IAuthenticationService
{
    Task<(string accessToken, string refreshToken)> AuthenticateAsync(string email, string password);

    Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken);
}
