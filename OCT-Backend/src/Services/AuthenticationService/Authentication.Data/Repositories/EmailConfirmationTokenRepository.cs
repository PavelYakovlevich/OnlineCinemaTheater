using Authentication.Contract.Repositories;
using Authentication.Data.Context;
using Authentication.Data.Entities;
using Authentication.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Repositories;

public class EmailConfirmationTokenRepository : IEmailConfirmationTokenRepository
{
    private readonly AuthenticationServiceDbContext _context;
    private readonly IMapper _mapper;

    public EmailConfirmationTokenRepository(AuthenticationServiceDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EmailConfirmationTokenModel> ReadByTokenValueAsync(string token)
    {
        var tokenEntity = await _context.EmailConfirmationTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Token == token);

        return _mapper.Map<EmailConfirmationTokenModel>(tokenEntity);
    }

    public async Task<EmailConfirmationTokenModel> CreateAsync(EmailConfirmationTokenModel tokenModel)
    {
        var token = _mapper.Map<EmailConfirmationToken>(tokenModel);

        await _context.EmailConfirmationTokens.AddAsync(token);

        await _context.SaveChangesAsync();

        return _mapper.Map<EmailConfirmationTokenModel>(token);
    }
}