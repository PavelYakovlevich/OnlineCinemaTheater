using Authentication.Contract.Repositories;
using Authentication.Data.Context;
using Authentication.Data.Entities;
using Authentication.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AuthenticationServiceDbContext _context;
    private readonly IMapper _mapper;

    public AccountRepository(AuthenticationServiceDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AccountModel> CreateAsync(AccountModel account)
    {
        var accountEntity = _mapper.Map<Account>(account);

        await _context.Accounts.AddAsync(accountEntity);

        await _context.SaveChangesAsync();

        _mapper.Map(accountEntity, account);

        return account;
    }

    public async Task<AccountModel> ReadByIdAsync(Guid id)
    {
        var accountEntity = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        return _mapper.Map<AccountModel>(accountEntity);
    }

    public async Task<AccountModel> ReadByEmailAsync(string email)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Email == email);

        return _mapper.Map<AccountModel>(account);
    }

    public async Task<AccountModel> ReadByCredentialsAsync(string email, string password)
    {
        var accountEntity = await _context.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Email == email && a.Password == password);

        return _mapper.Map<AccountModel>(accountEntity);
    }

    public async Task<bool> UpdateAsync(Guid id, AccountModel account)
    {
        var accountEntity = await _context.Accounts.SingleOrDefaultAsync(a => a.Id == id);

        if (accountEntity is null)
        {
            return false;
        }

        _mapper.Map(account, accountEntity);

        await _context.SaveChangesAsync();

        return true;
    }
}
