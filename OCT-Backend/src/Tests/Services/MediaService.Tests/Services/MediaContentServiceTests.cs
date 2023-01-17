using BlobStorage.Abstractions;
using Exceptions;
using FluentAssertions;
using MassTransit;
using Media.Contract.Repositories;
using Media.Contract.Services;
using Media.Core;
using Media.Domain;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MediaService.Tests.Services;

[TestFixture]
internal class MediaContentServiceTests
{
	private readonly IMediaContentService _service;
	private readonly Mock<BlobsServiceBase<IFormFile>> _blobStorageMock;
	private readonly Mock<IMediaRepository> _mediaRepositoryMock;
	private readonly Mock<IMediaInfoRepository> _mediaInfoRepositoryMock;

	public MediaContentServiceTests()
	{
		_blobStorageMock = new Mock<BlobsServiceBase<IFormFile>>();
		_mediaRepositoryMock = new Mock<IMediaRepository>();
		_mediaInfoRepositoryMock = new Mock<IMediaInfoRepository>();
        _service = new MediaContentService(_blobStorageMock.Object, _mediaRepositoryMock.Object, _mediaInfoRepositoryMock.Object);
	}

	[Test]
	public async Task CreateMediaContentAsync_MediaInfoRepository_ReadMediaInfoAsync_WasCalled_Once()
	{
		// Arrange
		var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
		var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

		_mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
			.ReturnsAsync(mediaInfoModel);

		_mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
			.Returns(Array.Empty<MediaContentModel>().ToAsyncEnumerable());

        // Act
        await _service.CreateMediaContentAsync(mediaContentModels.First());

		// Arrange
		_mediaInfoRepositoryMock.Verify(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default), Times.Once);
	}

    [Test]
    public void CreateMediaContentAsync_ExistingMediaId_ThrowsBadRequestException()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();
        mediaInfoModel.IsTvSerias = false;

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(mediaContentModels.ToAsyncEnumerable());

		// Act
		// Arrange
		Assert.ThrowsAsync<BadRequestException>(() => _service.CreateMediaContentAsync(mediaContentModels.First()));
    }

    [Test]
    public async Task CreateMediaContentAsync_ValidModel_AllModifiableFieldsWereUpdated()
    {
        // Arrange
        var mediaContentModel = Fakers.MediaContentModelFaker.Generate();
		mediaContentModel.Id = default;
		mediaContentModel.IssueDate = default;
		var oldNumberPropValue = mediaContentModel.Number;

        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(Array.Empty<MediaContentModel>().ToAsyncEnumerable());

        // Act
        await _service.CreateMediaContentAsync(mediaContentModel);

		// Arrange
		mediaContentModel.Id.Should().NotBe(default(Guid));
		mediaContentModel.IssueDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
		mediaContentModel.Number.Should().Be(oldNumberPropValue - 1);
    }

    [Test]
    public async Task CreateMediaContentAsync_MediaRepository_CreateAsync_WasCalled_Once()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(Array.Empty<MediaContentModel>().ToAsyncEnumerable());

        // Act
        await _service.CreateMediaContentAsync(mediaContentModels.First());

        // Arrange
        _mediaRepositoryMock.Verify(_ => _.CreateAsync(It.IsAny<MediaContentModel>(), default), Times.Once);
    }

    [Test]
    public async Task GetMediaContentAsync_MediaRepository_ReadAllByMediaIdAsync_WasCalled_Once()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(Array.Empty<MediaContentModel>().ToAsyncEnumerable());

        // Act
        await _service.GetMediaContentAsync(id)
            .AnyAsync();

        // Arrange
        _mediaRepositoryMock.Verify(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Test]
    public void GetMediaContentAsync_UnexistingMediaId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns((IAsyncEnumerable<MediaContentModel>)null);

        // Act
        // Arrange
        Assert.ThrowsAsync<ResourceNotFoundException>(async () => await _service.GetMediaContentAsync(id).FirstAsync());
    }

    [Test]
    public async Task DeleteMediaContentAsync_MediaInfoRepository_ReadMediaInfoAsync_WasCalled_Once()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(mediaContentModels.ToAsyncEnumerable());

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.DeleteMediaContentAsync(id);

        // Arrange
        _mediaInfoRepositoryMock.Verify(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Test]
    public void DeleteMediaContentAsync_UnexistingMediaInfoId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((MediaInfoModel)null);

        // Act
        // Arrange
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteMediaContentAsync(id));
    }

    [Test]
    public async Task DeleteMediaContentAsync_MediaRepository_ReadAllByMediaIdAsync_WasCalled_Once()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(mediaContentModels.ToAsyncEnumerable());

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.DeleteMediaContentAsync(id);

        // Arrange
        _mediaRepositoryMock.Verify(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Test]
    public void DeleteMediaContentAsync_UnexistingMediaId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(Array.Empty<MediaContentModel>().ToAsyncEnumerable());

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        // Arrange
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteMediaContentAsync(id));
    }

    [Test]
    public async Task DeleteMediaContentAsync_MediaRepository_DeleteByIdAsync_WasCalled_Once()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(mediaContentModels.ToAsyncEnumerable());

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.DeleteMediaContentAsync(id);

        // Arrange
        _mediaRepositoryMock.Verify(_ => _.DeleteByIdAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Test]
    public async Task DeleteMediaContentAsync_BlobStorage_DeleteBlobAsync_WasCalled_Once()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(mediaContentModels.ToAsyncEnumerable());

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.DeleteMediaContentAsync(id);

        // Arrange
        _blobStorageMock.Verify(_ => _.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public async Task GetMediaContentFileAsync_MediaInfoRepository_ReadMediaInfoAsync_WasCalled_Once()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(mediaContentModels.ToAsyncEnumerable());

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.GetMediaContentFileAsync(id);

        // Arrange
        _mediaInfoRepositoryMock.Verify(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Test]
    public void GetMediaContentFileAsync_UnexistingMediaInfoId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((MediaInfoModel)null);

        // Act
        // Arrange
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetMediaContentFileAsync(id));
    }

    [Test]
    public async Task GetMediaContentFileAsync_MediaRepository_ReadAllByMediaIdAsync_WasCalled_Once()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(mediaContentModels.ToAsyncEnumerable());

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.GetMediaContentFileAsync(id);

        // Arrange
        _mediaRepositoryMock.Verify(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Test]
    public void GetMediaContentFileAsync_UnexistingMediaId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var mediaContentModels = Fakers.MediaContentModelFaker.Generate(2);
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadAllByMediaIdAsync(It.IsAny<Guid>(), default))
            .Returns(Array.Empty<MediaContentModel>().ToAsyncEnumerable());

        _mediaInfoRepositoryMock.Setup(_ => _.ReadMediaInfoAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(mediaInfoModel);

        // Act
        // Arrange
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetMediaContentFileAsync(id));
    }

    [TearDown]
	public void ClearMocksInvocations()
	{
		_mediaInfoRepositoryMock.Invocations.Clear();
		_mediaRepositoryMock.Invocations.Clear();
		_blobStorageMock.Invocations.Clear();
	}
}
