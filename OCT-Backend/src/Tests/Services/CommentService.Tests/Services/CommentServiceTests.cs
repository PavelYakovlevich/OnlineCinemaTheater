using Comment.Contract.Repositories;
using Comment.Contract.Services;
using Comment.Domain;
using Exceptions;
using FluentAssertions;
using Moq;
using CommentServiceCore = Comment.Core.CommentService;

namespace CommentService.Tests.Services;

[TestFixture]
internal class CommentServiceTests
{
	private readonly Mock<ICommentRepository> _commentRepoMock;
	private readonly Mock<IUserRepository> _userRepoMock;
	private readonly ICommentService _service;

	public CommentServiceTests()
	{
		_commentRepoMock = new Mock<ICommentRepository>();
		_userRepoMock = new Mock<IUserRepository>();

        _service = new CommentServiceCore(_commentRepoMock.Object, _userRepoMock.Object);
    }

	[Test]
	public async Task CreateAsync_UserRepository_ReadByIdAsync_WasCalled_Once()
	{
		// Arrange
		var userModel = Fakers.UserModelFaker.Generate();
		var commentModel = Fakers.CommentModelFaker.Generate();

		_userRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>(), default))
			.ReturnsAsync(userModel);

		// Act
		await _service.CreateAsync(commentModel);

		// Assert
		_userRepoMock.Verify(_ => _.ReadByIdAsync(It.IsAny<Guid>(), default), Times.Once);
	}

    [Test]
    public void CreateAsync_InvalidUserId_ThrowsNotFoundException()
    {
        // Arrange
        var commentModel = Fakers.CommentModelFaker.Generate();

        _userRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((UserModel)null);

		// Act
		// Assert
		Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.CreateAsync(commentModel));
    }

    [Test]
    public void CreateAsync_CommentIssueDate_WasUpdatedToUtcNow()
    {
        // Arrange
		var userModel = Fakers.UserModelFaker.Generate();
        var commentModel = Fakers.CommentModelFaker.Generate();
        commentModel.IssueDate = default;

        _userRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(userModel);

        // Act
        _service.CreateAsync(commentModel);

        // Assert
        commentModel.IssueDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Test]
    public async Task CreateAsync_CommentRepository_CreateAsync_WasCalled_Once()
    {
        // Arrange
        var userModel = Fakers.UserModelFaker.Generate();
        var commentModel = Fakers.CommentModelFaker.Generate();

        _userRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(userModel);

        // Act
        await _service.CreateAsync(commentModel);

        // Assert
        _commentRepoMock.Verify(_ => _.CreateAsync(It.IsAny<CommentModel>(), default), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_CommentRepository_DeleteAsync_WasCalled_Once()
    {
        // Arrange
        _commentRepoMock.Setup(_ => _.DeleteAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(true);

        var commentId = Guid.NewGuid();

        // Act
        await _service.DeleteAsync(commentId);

        // Assert
        _commentRepoMock.Verify(_ => _.DeleteAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Test]
    public void DeleteAsync_InvalidCommentId_ThrowsNotFoundException()
    {
        // Arrange
        _commentRepoMock.Setup(_ => _.DeleteAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(false);

        var commentId = Guid.NewGuid();

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteAsync(commentId));
    }

    [Test]
    public async Task GetByIdAsync_CommentRepository_DeleteAsync_WasCalled_Once()
    {
        // Arrange
        var commentModel = Fakers.CommentModelFaker.Generate();

        _commentRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(commentModel);

        var commentId = Guid.NewGuid();

        // Act
        _ = await _service.GetByIdAsync(commentId);

        // Assert
        _commentRepoMock.Verify(_ => _.ReadByIdAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Test]
    public void GetByIdAsync_InvalidCommentId_ThrowsNotFoundException()
    {
        // Arrange
        _commentRepoMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((CommentModel)null);

        var commentId = Guid.NewGuid();

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetByIdAsync(commentId));
    }

    [Test]
    public async Task GetCommentsAsync_CommentRepository_ReadAllAsync_WasCalled_Once()
    {
        // Arrange
        var commentModels = Fakers.CommentModelFaker.Generate(10).ToAsyncEnumerable();

        _commentRepoMock.Setup(_ => _.ReadAllAsync(It.IsAny<Guid>(), It.IsAny<CommentsFilters>()))
            .Returns(commentModels);

        var commentId = Guid.NewGuid();
        var filters = new CommentsFilters();

        // Act
        _ = await _service.GetCommentsAsync(commentId, filters).FirstAsync();

        // Assert
        _commentRepoMock.Verify(_ => _.ReadAllAsync(It.IsAny<Guid>(), It.IsAny<CommentsFilters>()), Times.Once);
    }

    [Test]
    public void GetCommentsAsync_InvalidCommentId_ThrowsNotFoundException()
    {
        // Arrange
        _commentRepoMock.Setup(_ => _.ReadAllAsync(It.IsAny<Guid>(), It.IsAny<CommentsFilters>()))
            .Returns((IAsyncEnumerable<CommentModel>)null);

        var commentId = Guid.NewGuid();
        var filters = new CommentsFilters();

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(async () => _ = await _service.GetCommentsAsync(commentId, filters).FirstAsync());
    }

    [Test]
    public async Task UpdateAsync_CommentRepository_UpdateAsync_WasCalled_Once()
    {
        // Arrange
        var commentModel = Fakers.CommentModelFaker.Generate();

        _commentRepoMock.Setup(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CommentModel>(), default))
            .ReturnsAsync(true);

        var commentId = Guid.NewGuid();

        // Act
        await _service.UpdateAsync(commentId, commentModel);

        // Assert
        _commentRepoMock.Verify(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CommentModel>(), default), Times.Once);
    }

    [Test]
    public void UpdateAsync_InvalidCommentId_ThrowsNotFoundException()
    {
        // Arrange
        _commentRepoMock.Setup(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CommentModel>(), default))
            .ReturnsAsync(false);

        var commentModel = Fakers.CommentModelFaker.Generate();

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.UpdateAsync(commentModel.Id, commentModel));
    }

    [TearDown]
	public void ClearMocksInvocations()
	{
		_commentRepoMock.Invocations.Clear();
		_userRepoMock.Invocations.Clear();
    }
}
