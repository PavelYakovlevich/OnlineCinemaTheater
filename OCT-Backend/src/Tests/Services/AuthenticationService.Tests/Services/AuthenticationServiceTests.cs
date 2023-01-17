using Authentication.Contract.Repositories;
using Authentication.Contract.Services;
using Authentication.Core.Configuration;
using Authentication.Core.Generators;
using Authentication.Domain.Models;
using Bogus;
using Exceptions.AuthentificationService;
using Moq;
using AuthService = Authentication.Core.Services.AuthenticationService;

namespace AuthenticationService.Tests.Services;

[TestFixture]
internal class AuthenticationServiceTests
{
    private readonly Faker _faker = new ();
    private readonly Mock<IRefreshTokenRepository> _tokenRepositoryMock;
    private readonly Mock<IAccountService> _accountSeriviceMock;
    private readonly AuthService _authenticationService;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public AuthenticationServiceTests()
	{
        _tokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _accountSeriviceMock = new Mock<IAccountService>();
        _applicationConfiguration = Fakers.ApplicationConfigurationFaker.Generate();

        _authenticationService = new AuthService(
            _accountSeriviceMock.Object,
            _tokenRepositoryMock.Object,
            new JWTTokenGenerator(Fakers.JWTConfigurationFaker.Generate()),
            _applicationConfiguration);
    }

    [Test]
    public async Task AuthenticateAsync_Calls_AccountService_GetAccountByCredentialsAsync_Once()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var accountModel = Fakers.AccountFaker.Generate();
        var refreshTokenModel = Fakers.TokenFaker.Generate();

        _accountSeriviceMock.Setup(_ => _.GetAccountByCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(accountModel);
        _tokenRepositoryMock.Setup(_ => _.CreateAsync(It.IsAny<RefreshTokenModel>()))
            .ReturnsAsync(refreshTokenModel);

        // Act
        await _authenticationService.AuthenticateAsync(email, password);

        // Assert
        _accountSeriviceMock.Verify(_ => _.GetAccountByCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task AuthenticateAsync_Calls_TokenRepository_CreateAsync_Once()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var accountModel = Fakers.AccountFaker.Generate();
        var refreshTokenModel = Fakers.TokenFaker.Generate();

        _accountSeriviceMock.Setup(_ => _.GetAccountByCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(accountModel);
        _tokenRepositoryMock.Setup(_ => _.CreateAsync(It.IsAny<RefreshTokenModel>()))
            .ReturnsAsync(refreshTokenModel);

        // Act
        await _authenticationService.AuthenticateAsync(email, password);

        // Assert
        _tokenRepositoryMock.Verify(_ => _.CreateAsync(It.IsAny<RefreshTokenModel>()), Times.Once());
    }

    [Test]
    public void RefreshAsync_UnexistingToken_Throws_RefreshTokenException()
    {
        // Arrange
        var token = Fakers.TokenFaker.Generate().Token;

        _tokenRepositoryMock.Setup(_ => _.GetByTokenValueAsync(It.IsAny<string>()))
            .ReturnsAsync((RefreshTokenModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<RefreshTokenException>(() => _authenticationService.RefreshAsync(token));
    }

    [Test]
    public async Task RefreshAsync_Calls_TokenRepository_GetByTokenValueAsync_Once()
    {
        // Arrange
        var token = Fakers.TokenFaker.Generate();
        _tokenRepositoryMock.Setup(_ => _.GetByTokenValueAsync(It.IsAny<string>()))
            .ReturnsAsync(token);

        // Act
        await _authenticationService.RefreshAsync(token.Token);

        // Assert
        _tokenRepositoryMock.Verify(_ => _.GetByTokenValueAsync(It.IsAny<string>()), Times.Once());
    }

    [Test]
    public void RefreshAsync_ExpiredToken_Throws_RefreshTokenException()
    {
        // Arrange
        var tokenModel = Fakers.TokenFaker.Generate();
        tokenModel.IssueDate = DateTime.UtcNow.AddMinutes(-_applicationConfiguration.RefreshTokenExpirationTimeInMinutes - 1);

        _tokenRepositoryMock.Setup(_ => _.GetByTokenValueAsync(It.IsAny<string>()))
            .ReturnsAsync(tokenModel);

        // Act
        // Assert
        Assert.ThrowsAsync<RefreshTokenException>(() => _authenticationService.RefreshAsync(tokenModel.Token));
    }

    [Test]
    public async Task RefreshAsync_Calss_TokenRepository_CreateAsync_Once()
    {
        // Arrange
        var tokenModel = Fakers.TokenFaker.Generate();
        var refreshTokenModel = Fakers.TokenFaker.Generate();

        _tokenRepositoryMock.Setup(_ => _.GetByTokenValueAsync(It.IsAny<string>()))
            .ReturnsAsync(tokenModel);
        _tokenRepositoryMock.Setup(_ => _.CreateAsync(It.IsAny<RefreshTokenModel>()))
            .ReturnsAsync(refreshTokenModel);

        // Act
        await _authenticationService.RefreshAsync(tokenModel.Token);

        // Assert
        _tokenRepositoryMock.Verify(_ => _.CreateAsync(It.IsAny<RefreshTokenModel>()), Times.Once);
    }

    [TearDown]
    public void Teardown()
    {
        _tokenRepositoryMock.Invocations.Clear();
        _accountSeriviceMock.Invocations.Clear();
    }
}
