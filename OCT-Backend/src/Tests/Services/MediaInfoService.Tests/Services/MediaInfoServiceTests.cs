using AutoMapper;
using BlobStorage.Abstractions;
using Consul.Filtering;
using Exceptions;
using FluentAssertions;
using Infrastructure.MessageBroker.Abstractions;
using MediaInfo.Contract.Repositories;
using MediaInfo.Contract.Services;
using MediaInfo.Domain.MediaInfo;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework.Constraints;
using MediaInfoServiceCore = MediaInfo.Core.Services.MediaInfoService;

namespace MediaInfoService.Tests.Services;

[TestFixture]
internal class MediaInfoServiceTests
{
	private readonly IMediaInfoService _service;
    private readonly Mock<BlobsServiceBase<IFormFile>> _blobStorageMock;
    private readonly Mock<IMediaInfoRepository> _mediaRepositoryMock;
    private readonly Mock<IMediaInfoPublishService> _publishServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    public MediaInfoServiceTests()
	{
        _blobStorageMock = new Mock<BlobsServiceBase<IFormFile>>();
        _mediaRepositoryMock = new Mock<IMediaInfoRepository>();
        _publishServiceMock = new Mock<IMediaInfoPublishService>();
        _mapperMock = new Mock<IMapper>();
        
        _service = new MediaInfoServiceCore(_mediaRepositoryMock.Object, _blobStorageMock.Object, _publishServiceMock.Object, _mapperMock.Object);
	}

    [Test]
    public async Task CreateAsync_MediaInfoRepository_CreateAsync_WasCalled_Once()
    {
        // Arrange
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        // Act
        await _service.CreateAsync(mediaInfoModel);

        // Assert
        _mediaRepositoryMock.Verify(_ => _.CreateAsync(It.IsAny<MediaInfoModel>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ValidModel_MediaInfoCreatedMessageWasPublished()
    {
        // Arrange
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        // Act
        await _service.CreateAsync(mediaInfoModel);

        // Assert
        _publishServiceMock.Verify(_ => _.PublishMediaInfoCreatedMessage(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ValidModel_NameAndCountryAreUpperCased()
    {
        // Arrange
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        // Act
        await _service.CreateAsync(mediaInfoModel);

        // Assert
        mediaInfoModel.Name.Should().BeUpperCased();
        mediaInfoModel.Country.Should().BeUpperCased();
    }

    [Test]
    public void DeleteAsync_MediaInfoRepository_DeleteAsync_WasCalled_Once()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mediaRepositoryMock.Setup(_ => _.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        _service.DeleteAsync(id);

        // Assert
        _mediaRepositoryMock.Verify(_ => _.DeleteAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void DeleteAsync_UnexistingMediaInfoId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mediaRepositoryMock.Setup(_ => _.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteAsync(It.IsAny<Guid>()));
    }

    [Test]
    public async Task DeleteAsync_ValidModel_MediaInfoDeletedMessageWasPublished()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mediaRepositoryMock.Setup(_ => _.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _publishServiceMock.Verify(_ => _.PublishMediaInfoDeletedMessage(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public async Task DeletePictureAsync_MediaInfoRepository_ReadByIdAsync_WasCalled_Once()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaRepositoryMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.DeletePictureAsync(id);

        // Assert
        _mediaRepositoryMock.Verify(_ => _.ReadByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void DeletePictureAsync_UnexistingMediaInfoId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((MediaInfoModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeletePictureAsync(It.IsAny<Guid>()));
    }

    [Test]
    public async Task DeletePictureAsync_BlobStorage_DeleteBlobAsync_WasCalled_Once()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaRepositoryMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(mediaInfoModel);

        // Act
        await _service.DeletePictureAsync(id);

        // Assert
        _blobStorageMock.Verify(_ => _.DeleteBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_MediaInfoRepository_ReadAllAsync_WasCalled_Once()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediaInfoModels = Fakers.MediaInfoModelFaker.Generate(10);
        var filter = Fakers.MediaInfoFiltersModelFaker.Generate();

        _mediaRepositoryMock.Setup(_ => _.ReadAllAsync(It.IsAny<MediaInfoFiltersModel>()))
            .Returns(mediaInfoModels.ToAsyncEnumerable());

        // Act
        await _service.GetAllAsync(filter).AnyAsync();

        // Assert
        _mediaRepositoryMock.Verify(_ => _.ReadAllAsync(It.IsAny<MediaInfoFiltersModel>()), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_ValidFiltersModel_AllStringFilterValuesAreUpperCased()
    {
        // Arrange
        var id = Guid.NewGuid();
        var filter = Fakers.MediaInfoFiltersModelFaker.Generate();

        _mediaRepositoryMock.Setup(_ => _.ReadAllAsync(It.IsAny<MediaInfoFiltersModel>()))
            .Returns(Array.Empty<MediaInfoModel>().ToAsyncEnumerable());

        // Act
        await _service.GetAllAsync(filter).AnyAsync();

        // Assert
        filter.NameStartsWith.Should().BeUpperCased();
        filter.Country.Should().BeUpperCased();
        filter.Genres.All(g => g.All(c => char.IsLetter(c) && char.IsUpper(c))).Should().BeTrue();
    }

    [Test]
    public void GetByIdAsync_MediaInfoRepository_ReadByIdAsync_WasCalled_Once()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaRepositoryMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(mediaInfoModel);

        // Act
        _service.GetByIdAsync(id);

        // Assert
        _mediaRepositoryMock.Verify(_ => _.ReadByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void GetByIdAsync_UnexistingMediaInfoId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((MediaInfoModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetByIdAsync(id));
    }

    [Test]
    public void GetPictureAsync_BlobStorage_GetBlobAsync_WasCalled_Once()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dummyStream = new MemoryStream();

        _blobStorageMock.Setup(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(dummyStream);

        // Act
        _service.GetPictureAsync(id);

        // Assert
        _blobStorageMock.Verify(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void GetPictureAsync_UnexistingBlobId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _blobStorageMock.Setup(_ => _.GetBlobAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync((Stream)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetPictureAsync(id));
    }

    [Test]
    public async Task UpdateAsync_ValidModel_NameAndCountryAreUpperCased()
    {
        // Arrange
        var model = Fakers.PartialUpdateMediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PartialUpdateMediaInfoModel>()))
            .ReturnsAsync(true);

        // Act
        await _service.UpdateAsync(id, model);

        // Assert
        model.Name.Should().BeUpperCased();
        model.Country.Should().BeUpperCased();
    }

    [Test]
    public void UpdateAsync_MediaInfoRepository_UpdateAsync_WasCalled_Once()
    {
        // Arrange
        var model = Fakers.PartialUpdateMediaInfoModelFaker.Generate();

        var id = Guid.NewGuid();

        _mediaRepositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PartialUpdateMediaInfoModel>()))
            .ReturnsAsync(true);

        // Act
        _service.UpdateAsync(id, model);

        // Assert
        _mediaRepositoryMock.Verify(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PartialUpdateMediaInfoModel>()), Times.Once);
    }

    [Test]
    public void UpdateAsync_UnexistingMediaInfoId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = Fakers.PartialUpdateMediaInfoModelFaker.Generate();

        _mediaRepositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PartialUpdateMediaInfoModel>()))
            .ReturnsAsync(false);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.UpdateAsync(id, model));
    }

    [Test]
    public void UploadPictureAsync_MediaInfoModel_ReadByIdAsync_WasCalled_Once()
    {
        // Arrange
        var id = Guid.NewGuid();
        var formFile = new Mock<IFormFile>().Object;
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaRepositoryMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(mediaInfoModel);

        // Act
        _service.UploadPictureAsync(id, formFile);

        // Assert
        _mediaRepositoryMock.Verify(_ => _.ReadByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void UploadPictureAsync_UnexistingMediaInfo_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var formFile = new Mock<IFormFile>().Object;
        var mediaInfoModel = Fakers.MediaInfoModelFaker.Generate();

        _mediaRepositoryMock.Setup(_ => _.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((MediaInfoModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.UploadPictureAsync(id, formFile));
    }

    [TearDown]
    public void ClearMocksInvocations()
    {
        _blobStorageMock.Invocations.Clear();
        _mediaRepositoryMock.Invocations.Clear();
        _publishServiceMock.Invocations.Clear();
    }
}
