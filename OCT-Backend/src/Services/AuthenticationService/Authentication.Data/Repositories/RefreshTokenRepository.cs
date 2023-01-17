using Authentication.Contract.Repositories;
using Authentication.Data.Context;
using Authentication.Data.Entities;
using Authentication.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthenticationServiceDbContext _context;
    private readonly IMapper _mapper;

    public RefreshTokenRepository(AuthenticationServiceDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RefreshTokenModel> CreateAsync(RefreshTokenModel refreshTokenModel)
    {
        var refreshTokenEntity = _mapper.Map<RefreshToken>(refreshTokenModel);

        await _context.RefreshTokens.AddAsync(refreshTokenEntity);

        await _context.SaveChangesAsync();

        return _mapper.Map(refreshTokenEntity, refreshTokenModel);
    }

    public async Task<RefreshTokenModel> GetByTokenValueAsync(string token)
    {
        var tokenEntity = await _context.RefreshTokens
            .Include(rt => rt.Account)
            .AsNoTracking()
            .SingleOrDefaultAsync(rt => rt.Token == token);

        return _mapper.Map<RefreshTokenModel>(tokenEntity);
    }
}
