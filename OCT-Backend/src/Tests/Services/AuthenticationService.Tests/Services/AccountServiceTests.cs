using Authentication.Contract.Generators;
using Authentication.Contract.Repositories;
using Authentication.Contract.Services;
using Authentication.Core.Services;
using Authentication.Domain.Models;
using Bogus;
using Enums.AuthentificationService;
using Exceptions;
using Exceptions.AuthentificationService;
using FluentAssertions;
using Infrastructure.MessageBroker.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AuthenticationService.Tests.Services;

[TestFixture]
internal class AccountServiceTests
{
    private readonly Faker _faker = new ();
    private readonly IAccountService _accountService;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<IEmailConfirmationTokenRepository> _tokenRepoMock;
    private readonly Mock<ITokenGenerator<Guid>> _tokenGeneratorMock;
    private readonly Mock<IAuthenticationPublishService> _authenticationPublishServiceMock;

    public AccountServiceTests()
    {
        _accountRepoMock = new Mock<IAccountRepository>();
        _tokenRepoMock = new Mock<IEmailConfirmationTokenRepository>();
        _tokenGeneratorMock = new Mock<ITokenGenerator<Guid>>();
        _authenticationPublishServiceMock = new Mock<IAuthenticationPublishService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextAccessorMock.Setup(a => a.HttpContext.Request.Scheme)
            .Returns(_faker.Internet.Protocol());
        _httpContextAccessorMock.Setup(a => a.HttpContext.Request.Host)
            .Returns(new HostString(_faker.Internet.DomainName()));

        _accountService = new AccountService(
            _accountRepoMock.Object,
            _tokenRepoMock.Object,
            _tokenGeneratorMock.Object,
            Fakers.ApplicationConfigurationFaker.Generate(),
            _authenticationPublishServiceMock.Object);
    }

    [Test]
    public async Task ConfirmAccountEmailAsync_ValidToken_AccountEmailConfirmed()
    {
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();

        _accountRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(accountModel);
        _tokenRepoMock.Setup(_ => _.ReadByTokenValueAsync(It.IsAny<string>()))
            .ReturnsAsync(new EmailConfirmationTokenModel { AccountId = accountModel.Id });

        // Act
        await _accountService.ConfirmAccountEmailAsync(_faker.Random.Hash());

        // Assert
        accountModel.EmailConfirmed.Should().BeTrue();
    }

    [Test]
    public async Task ConfirmEmailAccount_Calls_AccountResository_UpdateAsync_Once()
    {
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();

        _accountRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(accountModel);
        _tokenRepoMock.Setup(_ => _.ReadByTokenValueAsync(It.IsAny<string>()))
            .ReturnsAsync(new EmailConfirmationTokenModel { AccountId = accountModel.Id });

        // Act
        await _accountService.ConfirmAccountEmailAsync(_faker.Random.Hash());

        // Assert
        _accountRepoMock.Verify(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<AccountModel>()), Times.Once());
    }

    [Test]
    public void ConfirmEmailAccount_InvalidToken_Throws_ResourceNotFoundException()
    {
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();

        _tokenRepoMock.Reset();

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _accountService.ConfirmAccountEmailAsync(_faker.Random.Hash()));
    }

    [Test]
    public void CreateAccountAsync_UsedEmail_Throws_AccountCreationException()
    {
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();
        var role = _faker.Random.Enum<AccountRole>();

        _accountRepoMock.Setup(_ => _.ReadByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(accountModel);

        // Act
        // Assert
        Assert.ThrowsAsync<AccountCreationException>(
            () => _accountService.CreateAccountAsync(accountModel, role));
    }

    [Test]
    public void CreateAccountAsync_Calls_UserRepository_CreateAsync_Once()
    {
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();
        var role = _faker.Random.Enum<AccountRole>();

        _accountRepoMock.Setup(_ => _.CreateAsync(It.IsAny<AccountModel>()))
            .ReturnsAsync(accountModel);

        // Act
        _accountService.CreateAccountAsync(accountModel, role);

        // Assert
        _accountRepoMock.Verify(_ => _.CreateAsync(It.IsAny<AccountModel>()), Times.Once);
    }

    [Test]
    public async Task CreateAccountAsync_Calls_TokenRepository_CreateAsync_Once()
    {
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();
        var role = _faker.Random.Enum<AccountRole>();

        _accountRepoMock.Setup(_ => _.CreateAsync(It.IsAny<AccountModel>()))
            .ReturnsAsync(accountModel);

        _tokenRepoMock.Setup(_ => _.CreateAsync(It.IsAny<EmailConfirmationTokenModel>()))
            .ReturnsAsync(new EmailConfirmationTokenModel());

        // Act
        await _accountService.CreateAccountAsync(accountModel, role);

        // Assert
        _tokenRepoMock.Verify(_ => _.CreateAsync(It.IsAny<EmailConfirmationTokenModel>()), Times.Once);
    }

    [Test]
    public async Task CreateAccountAsync_SendMessages_To_MessageBroker()
    {
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();
        var role = _faker.Random.Enum<AccountRole>();

        _accountRepoMock.Setup(_ => _.CreateAsync(It.IsAny<AccountModel>()))
            .ReturnsAsync(accountModel);

        _tokenRepoMock.Setup(_ => _.CreateAsync(It.IsAny<EmailConfirmationTokenModel>()))
            .ReturnsAsync(new EmailConfirmationTokenModel());

        // Act
        await _accountService.CreateAccountAsync(accountModel, role);

        // Assert
        _authenticationPublishServiceMock.Verify(
            _ => _.PublishAccountCreatedMessage(It.IsAny<Guid>()), Times.Once);
        _authenticationPublishServiceMock.Verify(
            _ => _.PublishSendConfirmationEmailMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAccountByCredentialsAsync_UnusedEmail_Throws_ResourceNotFoundException()
    {
        // Arrange
        _accountRepoMock.Setup(_ => _.ReadByCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((AccountModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => 
            _accountService.GetAccountByCredentialsAsync(_faker.Internet.Email(), _faker.Internet.Password()));
    }

    [Test]
    public async Task GetAccountByCredentialsAsync_Calls_AccountRepository_ReadByCreadentialsAsync_Once()
    {
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();
        _accountRepoMock.Setup(_ => _.ReadByCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(accountModel);

        // Act
        await _accountService.GetAccountByCredentialsAsync(_faker.Internet.Email(), _faker.Internet.Password());

        // Assert
        _accountRepoMock.Verify(_ => _.ReadByCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAccountByIdAsync_UnexistingId_Throws_AccountOperationException()
    {
        // Arrange
        _accountRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((AccountModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<AccountOperationException>(() => 
            _accountService.GetAccountByIdAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task GetAccountByIdAsync_Calls_AccountRepository_GetAccountByIdAsync_Once()
    {
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();
        _accountRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(accountModel);

        // Act
        await _accountService.GetAccountByIdAsync(Guid.NewGuid());

        // Assert
        _accountRepoMock.Verify(_ => _.ReadByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public async Task SendChangePasswordEmailAsync_Calls_AccountRepository_ReadByEmailAsync_Once()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var accountModel = Fakers.AccountFaker.Generate();
        _accountRepoMock.Setup(_ => _.ReadByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(accountModel);

        // Act
        await _accountService.SendChangePasswordEmailAsync(email);

        // Assert
        _accountRepoMock.Verify(_ => _.ReadByEmailAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task SendChangePasswordEmailAsync_Pass_SendPasswordChangeEmail_To_Broker_Once()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var accountModel = Fakers.AccountFaker.Generate();
        _accountRepoMock.Setup(_ => _.ReadByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(accountModel);

        // Act
        await _accountService.SendChangePasswordEmailAsync(email);

        // Assert
        _authenticationPublishServiceMock.Verify(_ => 
            _.PublishSendPasswordChangeEmailMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>()), 
            Times.Once);
    }

    [Test]
    public void UpdatePasswordAsync_UnexistingId_Throws_ResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var token = _faker.Random.String(10);
        var password = _faker.Internet.Password();

        _accountRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((AccountModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _accountService.UpdatePasswordAsync(id, token, password));
    }

    [Test]
    public async Task UpdatePasswordAsync_Calls_AccountRepository_ReadByIdAsync_Once()
    {
        // Arrange
        var id = Guid.NewGuid();
        var token = _faker.Random.String(10);
        var password = _faker.Internet.Password();

        var accountModel = Fakers.AccountFaker.Generate();
        _accountRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(accountModel);
        _tokenGeneratorMock.Setup(_ => _.ValidateToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(true);

        // Act
        await _accountService.UpdatePasswordAsync(id, token, password);

        // Assert
        _accountRepoMock.Verify(_ => _.ReadByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public async Task UpdatePasswordAsync_Calls_AccountRepository_UpdateAsync_Once()
    {
        // Arrange
        var id = Guid.NewGuid();
        var token = _faker.Random.String(10);
        var password = _faker.Internet.Password();

        var accountModel = Fakers.AccountFaker.Generate();
        _accountRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(accountModel);
        _tokenGeneratorMock.Setup(_ => _.ValidateToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(true);

        // Act
        await _accountService.UpdatePasswordAsync(id, token, password);

        // Assert
        _accountRepoMock.Verify(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<AccountModel>()), Times.Once);
    }

    [Test]
    public async Task UpdatePasswordAsync_ValidPasswordAndId_UpdatesPassword()
    {
        // Arrange
        var id = Guid.NewGuid();
        var token = _faker.Random.String(10);
        var password = _faker.Internet.Password();
        var accountModel = Fakers.AccountFaker.Generate();
        var oldPassword = accountModel.Password;

        _accountRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(accountModel);
        _tokenGeneratorMock.Setup(_ => _.ValidateToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(true);

        // Act
        await _accountService.UpdatePasswordAsync(id, token, password);

        // Assert
        oldPassword.Should().NotBe(accountModel.Password);
    }

    [TearDown]
    public void Teardown()
    {
        _tokenRepoMock.Invocations.Clear();
        _accountRepoMock.Invocations.Clear();
        _authenticationPublishServiceMock.Invocations.Clear();
    }
}
