using Authentication.Domain.Models;

namespace Authentication.Contract.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenModel> CreateAsync(RefreshTokenModel refreshTokenModel);

    Task<RefreshTokenModel> GetByTokenValueAsync(string token);
}