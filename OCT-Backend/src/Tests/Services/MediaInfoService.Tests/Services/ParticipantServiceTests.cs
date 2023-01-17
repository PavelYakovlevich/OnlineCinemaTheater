using BlobStorage.Abstractions;
using Exceptions;
using FluentAssertions;
using MediaInfo.Contract.Repositories;
using MediaInfo.Contract.Services;
using MediaInfo.Core.Services;
using MediaInfo.Domain.Participant;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MediaInfoService.Tests.Services;

[TestFixture]
internal class ParticipantServiceTests
{
	private readonly Mock<IParticipantRepository> _participantRepositoryMock;
	private readonly Mock<BlobsServiceBase<IFormFile>> _blobStorageMock;
	private readonly IParticipantService _service;

	public ParticipantServiceTests()
	{
		_participantRepositoryMock = new Mock<IParticipantRepository>();
		_blobStorageMock = new Mock<BlobsServiceBase<IFormFile>>();
		_service = new ParticipantService(_participantRepositoryMock.Object, _blobStorageMock.Object);
	}

	[Test]
	public void CreateAsync_ParticipantRepository_CreateAsync_WasCalled_Once()
	{
		// Arrange
		var participantModel = Fakers.ParticipantModelFaker.Generate();

		// Act
		_service.CreateAsync(participantModel);

		// Assert
		_participantRepositoryMock.Verify(_ => _.CreateAsync(It.IsAny<ParticipantModel>()), Times.Once);
	}

    [Test]
    public void CreateAsync_ValidParticipantModel_NameAndSurnameAndCountry_Uppercased()
    {
        // Arrange
        var participantModel = Fakers.ParticipantModelFaker.Generate();

        // Act
        _service.CreateAsync(participantModel);

		// Assert
		participantModel.Name.Should().BeUpperCased();
		participantModel.Surname.Should().BeUpperCased();
		participantModel.Country.Should().BeUpperCased();
    }

    [Test]
    public void GetByIdAsync_ParticipantRepository_ReadById_WasCalled_Once()
    {
        // Arrange
        var participantModel = Fakers.ParticipantModelFaker.Generate();
		var id = Guid.NewGuid();

        // Act
        _service.GetByIdAsync(id);

        // Assert
        _participantRepositoryMock.Verify(_ => _.ReadById(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void GetByIdAsync_UnexistingParticipantId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _participantRepositoryMock.Setup(_ => _.ReadById(It.IsAny<Guid>()))
            .ReturnsAsync((ParticipantModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetByIdAsync(id));
    }

    [Test]
    public void GetPictureAsync_ParticipantRepository_ReadById_WasCalled_Once()
    {
        // Arrange
        var participantModel = Fakers.ParticipantModelFaker.Generate();
        var id = Guid.NewGuid();

        // Act
        _service.GetPictureAsync(id);

        // Assert
        _participantRepositoryMock.Verify(_ => _.ReadById(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void GetPictureAsync_UnexistingParticipantId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _participantRepositoryMock.Setup(_ => _.ReadById(It.IsAny<Guid>()))
            .ReturnsAsync((ParticipantModel)null);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetPictureAsync(id));
    }

    [Test]
    public void GetPictureAsync_BlobStorage_GetBlobAsync_WasCalled_Once()
    {
        // Arrange
        var participantModel = Fakers.ParticipantModelFaker.Generate();
        var id = Guid.NewGuid();

        // Act
        _service.GetPictureAsync(id);

        // Assert
        _participantRepositoryMock.Verify(_ => _.ReadById(It.IsAny<Guid>()), Times.Once);
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
    public void UpdateAsync_ParticipantRepository_UpdateAsync_WasCalled_Once()
    {
        // Arrange
        var participantModel = Fakers.ParticipantModelFaker.Generate();

        _participantRepositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<ParticipantModel>()))
            .ReturnsAsync(true);

        // Act
        _service.UpdateAsync(participantModel.Id, participantModel);

        // Assert
        _participantRepositoryMock.Verify(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<ParticipantModel>()), Times.Once);
    }

    [Test]
    public void UpdateAsync_UnexistingParticipantId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var participantModel = Fakers.ParticipantModelFaker.Generate();

        _participantRepositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<Guid>(), It.IsAny<ParticipantModel>()))
            .ReturnsAsync(false);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.UpdateAsync(participantModel.Id, participantModel));
    }

    [Test]
    public void UpdateAsync_ValidParticipantModel_NameAndSurnameAndCountry_AreUppercased()
    {
        // Arrange
        var participantModel = Fakers.ParticipantModelFaker.Generate();

        // Act
        _service.UpdateAsync(participantModel.Id, participantModel);

        // Assert
        participantModel.Name.Should().BeUpperCased();
        participantModel.Surname.Should().BeUpperCased();
        participantModel.Country.Should().BeUpperCased();
    }

    [TearDown]
    public void ClearMocksInvocations()
    {
        _participantRepositoryMock.Invocations.Clear();
		_blobStorageMock.Invocations.Clear();
    }
}
