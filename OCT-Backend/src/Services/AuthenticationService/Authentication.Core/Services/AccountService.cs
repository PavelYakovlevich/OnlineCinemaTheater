using Authentication.Contract.Generators;
using Authentication.Contract.Repositories;
using Authentication.Contract.Services;
using Authentication.Core.Configuration;
using Authentication.Domain.Models;
using Enums.AuthentificationService;
using Exceptions;
using Exceptions.AuthentificationService;
using Infrastructure.MessageBroker.Abstractions;
using Messages.AuthenticationService;
using Messages.NotificationServices;
using Serilog;

namespace Authentication.Core.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly IEmailConfirmationTokenRepository _tokensRepository;
    private readonly ITokenGenerator<Guid> _tokenGenerator;
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly IAuthenticationPublishService _publishService;

    public AccountService(
        IAccountRepository repository,
        IEmailConfirmationTokenRepository tokensRepository,
        ITokenGenerator<Guid> tokenGenerator,
        ApplicationConfiguration applicationConfiguration,
        IAuthenticationPublishService publishService)
    {
        _repository = repository;
        _tokensRepository = tokensRepository;
        _tokenGenerator = tokenGenerator;
        _applicationConfiguration = applicationConfiguration;
        _publishService = publishService;
    }

    public async Task ConfirmAccountEmailAsync(string token)
    {
        var tokenModel = await _tokensRepository.ReadByTokenValueAsync(token) ??
            throw new ResourceNotFoundException($"Token {token} doesn't exist.");

        var accountModel = await _repository.ReadByIdAsync(tokenModel.AccountId);

        accountModel.EmailConfirmed = true;

        await _repository.UpdateAsync(accountModel.Id, accountModel);
    }

    public async Task<AccountModel> CreateAccountAsync(AccountModel accountModel, AccountRole accountRole)
    {
        if (await _repository.ReadByEmailAsync(accountModel.Email) is not null)
        {
            throw new AccountCreationException($"Account with email {accountModel.Email} exists.", accountModel.Email, accountModel.Id);
        }

        accountModel.Role = accountRole;
        accountModel.RegistrationDate = DateTime.UtcNow;
        accountModel.Password = EncryptString(accountModel.Password);

        accountModel = await _repository.CreateAsync(accountModel);

        var tokenModel = new EmailConfirmationTokenModel
        {
            Token = _tokenGenerator.GenerateToken(accountModel.Id),
            AccountId = accountModel.Id,
            IssueDate = DateTime.UtcNow,
        };

        await _tokensRepository.CreateAsync(tokenModel);
        Log.Debug("Email confirmation token '{token}' was generated", tokenModel.Token);

        await _publishService.PublishSendConfirmationEmailMessage(accountModel.Email, tokenModel.Token);
        Log.Debug("{message} was published", nameof(SendConfirmationEmailMessage));

        await _publishService.PublishAccountCreatedMessage(accountModel.Id);
        Log.Debug("{message} was published", nameof(CreateUserMessage));

        return accountModel;
    }

    public async Task<AccountModel> GetAccountByCredentialsAsync(string email, string password)
    {
        var accountModel = await _repository.ReadByCredentialsAsync(email, EncryptString(password)) ??
            throw new ResourceNotFoundException($"Invalid email {email} or password.");

        Log.Debug("Account with email: {email} was found.", email);

        return accountModel;
    }

    public async Task<AccountModel> GetAccountByIdAsync(Guid id)
    {
        var accountModel = await _repository.ReadByIdAsync(id) ??
            throw new AccountOperationException($"Account with id {id} doesn't exist.");

        Log.Debug("Account with id: {id} was found.", id);

        return accountModel;
    }

    public async Task SendChangePasswordEmailAsync(string email)
    {
        var accountModel = await _repository.ReadByEmailAsync(email);

        if (accountModel is null)
        {
            return;
        }

        var token = _tokenGenerator.GenerateToken(accountModel.Id);

        await _publishService.PublishSendPasswordChangeEmailMessage(email, accountModel.Id, token);
        Log.Debug("{message} was published", nameof(SendPasswordChangeEmailMessage));
    }

    public async Task UpdatePasswordAsync(Guid id, string token, string password)
    {
        var accountModel = await _repository.ReadByIdAsync(id) ??
            throw new ResourceNotFoundException($"Account with id {id} doesn't exist.");

        if (!_tokenGenerator.ValidateToken(id, token))
        {
            throw new BadRequestException($"Invalid token");
        }

        accountModel.Password = EncryptString(password);

        await _repository.UpdateAsync(id, accountModel);
             
        Log.Debug("Password of user with id {id} was updated", id);
    }

    private string EncryptString(string @string) =>
        BCrypt.Net.BCrypt.HashPassword(@string, _applicationConfiguration.Salt);
}