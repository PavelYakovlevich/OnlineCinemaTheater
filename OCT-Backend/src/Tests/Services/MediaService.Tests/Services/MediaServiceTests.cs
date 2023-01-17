using BlobStorage.Abstractions;
using Exceptions;
using Media.Contract.Repositories;
using Media.Contract.Services;
using Media.Domain;
using Microsoft.AspNetCore.Http;
using Moq;
using MediaServiceCore = Media.Core.MediaService;

namespace MediaService.Tests.Services;

[TestFixture]
internal class MediaServiceTests
{
	private readonly IMediaService _service;
	private readonly Mock<BlobsServiceBase<IFormFile>> _blobStorageMock;
	private readonly Mock<IMediaInfoRepository> _mediaInfoRepoMock;
	private readonly Mock<IMediaRepository> _mediaRepoMock;

	public MediaServiceTests()
	{
        _blobStorageMock = new Mock<BlobsServiceBase<IFormFile>>();
        _mediaInfoRepoMock = new Mock<IMediaInfoRepository>();
        _mediaRepoMock = new Mock<IMediaRepository>();

        _service = new MediaServiceCore(_blobStorageMock.Object, _mediaRepoMock.Object, _mediaInfoRepoMock.Object);
	}

    [Test]
    public async Task CreateMediaAsync_MediaRepository_CreateMediaInfoAsync_WasCalled_Once()
    {
        // Arrange
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        // Act
        await _service.CreateMediaAsync(mediaInfoModel);

        // Assert
        _mediaInfoRepoMock.Verify(_ => _.CreateMediaInfoAsync(It.IsAny<MediaInfoModel>(), default), Times.Once());
    }

    [Test]
    public async Task DeleteMediaAsync_MediaRepository_ReadAllByMediaIdAsync_WasCalled_Once()
    {
        // Arrange
        var mediaModels = Fakers.MediaContentModelFaker.Generate(100);
        var id = Guid.NewGuid();

        _mediaRepoMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(mediaModels.ToAsyncEnumerable());

        // Act
        await _service.DeleteMediaAsync(id);

        // Assert
        _mediaRepoMock.Verify(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default), Times.Once());
    }

    [Test]
    public async Task DeleteMediaAsync_MediaInfoRepository_DeleteMediaInfoAsync_WasCalled_Once()
    {
        // Arrange
        var mediaModels = Fakers.MediaContentModelFaker.Generate(100);
        var id = Guid.NewGuid();

        _mediaRepoMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(mediaModels.ToAsyncEnumerable());

        // Act
        await _service.DeleteMediaAsync(id);

        // Assert
        _mediaInfoRepoMock.Verify(_ => _.DeleteMediaInfoAsync(It.IsAny<Guid>(), default), Times.Once());
    }

    [Test]
    public void DeleteMediaAsync_UnexistingMediaInfoId_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mediaRepoMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns((IAsyncEnumerable<MediaContentModel>)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteMediaAsync(id));
    }

    [Test]
    public async Task UpdateMediaAsync_MediaInfoRepository_UpdateMediaInfoAsync_WasCalled_Once()
    {
        // Arrange
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaInfoRepoMock.Setup(
            _ => _.UpdateMediaInfoAsync(It.IsAny<Guid>(), It.IsAny<MediaInfoModel>(), default))
            .ReturnsAsync(true);

        // Act
        await _service.UpdateMediaAsync(mediaInfoModel.Id, mediaInfoModel);

        // Assert
        _mediaInfoRepoMock.Verify(
            _ => _.UpdateMediaInfoAsync(It.IsAny<Guid>(), It.IsAny<MediaInfoModel>(), default), 
            Times.Once);
    }

    [Test]
    public void UpdateMediaAsync_UnexistingMediaInfoId_ThrowsNotFoundException()
    {
        // Arrange
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaInfoRepoMock.Setup(
            _ => _.UpdateMediaInfoAsync(It.IsAny<Guid>(), It.IsAny<MediaInfoModel>(), default))
            .ReturnsAsync(false);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _service.UpdateMediaAsync(mediaInfoModel.Id, mediaInfoModel));
    }

    [TearDown]
    public void ClearMocksInvocations()
    {
        _mediaRepoMock.Invocations.Clear();
        _blobStorageMock.Invocations.Clear();
        _mediaInfoRepoMock.Invocations.Clear();
    }
}
