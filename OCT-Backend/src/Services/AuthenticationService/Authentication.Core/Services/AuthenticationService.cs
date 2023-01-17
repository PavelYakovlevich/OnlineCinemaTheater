using Authentication.Contract.Repositories;
using Authentication.Contract.Services;
using Authentication.Core.Configuration;
using Authentication.Core.Generators;
using Authentication.Domain.Models;
using Exceptions.AuthentificationService;

namespace Authentication.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAccountService _accountService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JWTTokenGenerator _jWTTokenGenerator;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public AuthenticationService(IAccountService accountService,
        IRefreshTokenRepository refreshTokenRepository,
        JWTTokenGenerator jWTTokenGenerator,
        ApplicationConfiguration applicationConfiguration)
    {
        _accountService = accountService;
        _jWTTokenGenerator = jWTTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _applicationConfiguration = applicationConfiguration;
    }

    public async Task<(string accessToken, string refreshToken)> AuthenticateAsync(string email, string password)
    {
        var accountModel = await _accountService.GetAccountByCredentialsAsync(email, password);

        return await CreateTokensAsync(accountModel);
    }

    public async Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken)
    {
        var refreshTokenModel = await _refreshTokenRepository.GetByTokenValueAsync(refreshToken) ??
            throw new RefreshTokenException($"Token {refreshToken} is invalid or doesn't exist.", refreshToken);

        if (refreshTokenModel.IssueDate.AddMinutes(_applicationConfiguration.RefreshTokenExpirationTimeInMinutes) < DateTime.UtcNow)
        {
            throw new RefreshTokenException($"Provided token {refreshToken} is expired.");
        }

        return await CreateTokensAsync(refreshTokenModel.Account); 
    }

    private async Task<(string accessToken, string refreshToken)> CreateTokensAsync(AccountModel accountModel)
    {
        var newRefreshTokenModel = new RefreshTokenModel
        {
            Token = RefreshTokenGenerator.GenerateToken(_applicationConfiguration.TokenLength),
            IssueDate = DateTime.UtcNow,
            AccountId = accountModel.Id,
        };

        newRefreshTokenModel = await _refreshTokenRepository.CreateAsync(newRefreshTokenModel);

        var accessToken = _jWTTokenGenerator.GenerateToken(accountModel);

        return (accessToken, newRefreshTokenModel.Token);
    }
}
