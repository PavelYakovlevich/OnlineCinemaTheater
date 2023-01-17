using BlobStorage.Abstractions;
using Exceptions.UserService;
using FluentAssertions;
using Infrastructure.MessageBroker.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using User.Contract.Repositories;
using User.Contract.Services;
using User.Domain.Models;
using UserServiceCore = User.Core.Services.UserService;

namespace UserService.Tests.Services;

[TestFixture]
internal class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<BlobsServiceBase<IFormFile>> _blobServiceMock;
    private readonly Mock<IUserPublishService> _publishServiceMock;
    private readonly IUserService _service;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _blobServiceMock = new Mock<BlobsServiceBase<IFormFile>>();
        _publishServiceMock = new Mock<IUserPublishService>();
        _service = new UserServiceCore(_userRepoMock.Object, _blobServiceMock.Object, _publishServiceMock.Object);
    }

    [Test]
    public async Task CreateUserAsync_Calls_UserRepository_CreateUserAsync_Once()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _service.CreateUserAsync(userId);

        // Assert
        _userRepoMock.Verify(_ => _.CreateUserAsync(It.IsAny<UserModel>()), Times.Once);
    }

    [Test]
    public void GetUserByIdAsync_UnexistingId_ThrowsUserOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync((UserModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<UserOperationException>(() => _service.GetUserByIdAsync(userId));
    }

    [Test]
    public async Task GetUserByIdAsync_Calls_UserRepository_GetUserById_Once()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(userModel);

        // Act
        await _service.GetUserByIdAsync(userId);

        // Assert
        _userRepoMock.Verify(_ => _.GetUserById(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void UpdateUserAsync_UnexistingId_ThrowsUserOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();

        _userRepoMock.Setup(_ => _.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<UserModel>()))
            .ReturnsAsync(false);

        // Act
        // Assert
        Assert.ThrowsAsync<UserOperationException>(() => _service.UpdateUserAsync(userId, userModel));
    }

    [Test]
    public async Task UpdateUserAsync_Calls_UserRepository_UpdateUserAsync_Once()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();

        _userRepoMock.Setup(_ => _.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<UserModel>()))
            .ReturnsAsync(true);

        // Act
        await _service.UpdateUserAsync(userId, userModel);

        // Assert
        _userRepoMock.Verify(_ => _.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<UserModel>()), Times.Once);
    }

    [Test]
    public void UploadUserPhoto_UnexistingId_ThrowsUserOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync((UserModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<UserOperationException>(() => _service.UploadUserPhoto(userId, null));
    }

    [Test]
    public async Task UploadUserPhoto_Calls_UserRepository_GetUserById_Once()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(userModel);
        _userRepoMock.Setup(_ => _.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<UserModel>()))
            .ReturnsAsync(true);

        // Act
        await _service.UploadUserPhoto(userId, null);

        // Assert
        _userRepoMock.Verify(_ => _.GetUserById(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public async Task DeleteUserPhotoAsync_ValidId_PhotoLinkIsSetToNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(userModel);

        // Act
        await _service.DeleteUserPhotoAsync(userId);

        // Assert
        userModel.PhotoLink.Should().BeNull();
    }

    [Test]
    public async Task DeleteUserPhotoAsync_Calls_BlobService_DeleteBlobAsync_Once()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(userModel);

        // Act
        await _service.DeleteUserPhotoAsync(userId);

        // Assert
        _blobServiceMock.Verify(
            _ => _.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()),
            Times.Once);
    }

    [Test]
    public async Task DeleteUserPhotoAsync_Calls_UserRepository_UpdateUserAsync_Once()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(userModel);

        // Act
        await _service.DeleteUserPhotoAsync(userId);

        // Assert
        _userRepoMock.Verify(_ => _.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<UserModel>()), Times.Once);
    }

    [Test]
    public async Task GetUserPhotoAsync_Calls_UserRepository_GetUserById_Once()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();
        using var memoryStream = new MemoryStream();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(userModel);
        _blobServiceMock.Setup(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(memoryStream);

        // Act
        await _service.GetUserPhotoAsync(userId);

        // Assert
        _userRepoMock.Verify(_ => _.GetUserById(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void GetUserPhotoAsync_UnexistingId_ThrowsUserOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync((UserModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<UserOperationException>(() => _service.GetUserPhotoAsync(userId));
    }

    [Test]
    public void GetUserPhotoAsync_UnexistingId_ThrowsBlobOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(userModel);
        _blobServiceMock.Setup(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync((Stream)null);

        // Act
        // Assert
        Assert.ThrowsAsync<BlobOperationException>(() => _service.GetUserPhotoAsync(userId));
    }

    [Test]
    public async Task GetUserPhotoAsync_Calls_BlobService_GetBlobAsync_Once()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = Fakers.UserModelFaker.Generate();
        using var memoryStream = new MemoryStream();

        _userRepoMock.Setup(_ => _.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(userModel);
        _blobServiceMock.Setup(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(memoryStream);

        // Act
        await _service.GetUserPhotoAsync(userId);

        // Assert
        _blobServiceMock.Verify(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
    }

    [TearDown]
    public void ClearInvocationLists()
    {
        _userRepoMock.Invocations.Clear();
        _blobServiceMock.Invocations.Clear();
    }
}
