using BlobStorage.Abstractions;
using Castle.DynamicProxy;
using Exceptions;
using Media.Contract.Repositories;
using Media.Core;
using Media.Domain;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MediaService.Tests.Services;

[TestFixture]
internal class TrailerServiceTests
{
	private readonly TrailerService _service;
	private readonly Mock<BlobsServiceBase<IFormFile>> _blobStorageMock;
	private readonly Mock<IMediaInfoRepository> _mediaInfoRepoMock;

	public TrailerServiceTests()
	{
		_blobStorageMock = new Mock<BlobsServiceBase<IFormFile>>();
		_mediaInfoRepoMock = new Mock<IMediaInfoRepository>();

		_service = new TrailerService(_blobStorageMock.Object, _mediaInfoRepoMock.Object);
	}

	[Test]
	public async Task DeleteTrailerAsync_MediaInfoRepository_ReadMediaInfoAsync_WasCalled_Once()
	{
		// Arrange 
		var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

		_mediaInfoRepoMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
			.ReturnsAsync(mediaInfoModel);

		// Act
		await _service.DeleteTrailerAsync(mediaInfoModel.Id);

		// Assert
		_mediaInfoRepoMock.Verify(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default), Times.Once);
	}

    [Test]
    public void DeleteTrailerAsync_UnexistingMediaId_ThrowsNotFoundException()
    {
		// Arrange 
		var mediaInfoId = Guid.NewGuid();

        _mediaInfoRepoMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((MediaInfoModel)null);

        // Act
		// Assert
		Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteTrailerAsync(mediaInfoId));
    }

    [Test]
    public async Task DeleteTrailerAsync_BlobStorage_DeleteBlobAsync_WasCalled_Once()
    {
        // Arrange 
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaInfoRepoMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.DeleteTrailerAsync(mediaInfoModel.Id);

        // Assert
        _blobStorageMock.Verify(_ => _.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public async Task GetTrailerAsync_BlobStorage_GetBlobAsync_WasCalled_Once()
    {
        // Arrange 
        using var dummyStream = new MemoryStream();
        var id = Guid.NewGuid();

        _blobStorageMock.Setup(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(dummyStream);

        // Act
        await _service.GetTrailerAsync(id);

        // Assert
        _blobStorageMock.Verify(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void GetTrailerAsync_UnexistingMediaId_ThrowsNotFoundException()
    {
        // Arrange
        _blobStorageMock.Setup(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync((Stream)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetTrailerAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task UploadTrailerAsync_MediaInfoRepository_ReadMediaInfoAsync_WasCalled_Once()
    {
        // Arrange 
        var formFile = new Mock<IFormFile>().Object;
        var id = Guid.NewGuid();
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaInfoRepoMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.UploadTrailerAsync(id, formFile);

        // Assert
        _mediaInfoRepoMock.Verify(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Test]
    public void UploadTrailerAsync_UnexistingMediaInfoId_ThrowsNotFoundException()
    {
        // Arrange 
        var formFile = new Mock<IFormFile>().Object;
        var id = Guid.NewGuid();

        _mediaInfoRepoMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((MediaInfoModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.UploadTrailerAsync(id, formFile));
    }

    [TearDown]
	public void ClearMocksInvocations()
	{
		_blobStorageMock.Invocations.Clear();
        _mediaInfoRepoMock.Invocations.Clear();
    }
}
