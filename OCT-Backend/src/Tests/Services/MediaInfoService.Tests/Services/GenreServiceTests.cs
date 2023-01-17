using Exceptions;
using FluentAssertions;
using MediaInfo.Contract.Repositories;
using MediaInfo.Contract.Services;
using MediaInfo.Core.Services;
using MediaInfo.Domain.Genre;
using Moq;

namespace MediaInfoService.Tests.Services;

[TestFixture]
internal class GenreServiceTests
{
	private readonly Mock<IGenreRepository> _genreRepositoryMock;
	private readonly IGenreService _service;

	public GenreServiceTests()
	{
		_genreRepositoryMock = new Mock<IGenreRepository>();

		_service = new GenreService(_genreRepositoryMock.Object);
	}

	[Test]
	public async Task CreateGenreAsync_ValidModel_GenreNameWasUppercased()
	{
		// Arrange 
		var genreModel = Fakers.GenreModelFaker.Generate();
		var genreNameOldValue = genreModel.Name;

		// Act
		await _service.CreateGenreAsync(genreModel);

		// Assert
		genreModel.Name.Should().Be(genreNameOldValue.ToUpper());
	}

    [Test]
    public async Task CreateGenreAsync_GenreRepository_CreateAsync_WasCalled_Once()
    {
        // Arrange 
        var genreModel = Fakers.GenreModelFaker.Generate();

        // Act
        await _service.CreateGenreAsync(genreModel);

		// Assert
		_genreRepositoryMock.Verify(_ => _.CreateAsync(It.IsAny<GenreModel>()), Times.Once);
    }

    [Test]
    public async Task DeleteGenreAsync_GenreRepository_DeleteAsync_WasCalled_Once()
    {
		// Arrange 
		var id = Guid.NewGuid();

		_genreRepositoryMock.Setup(_ => _.DeleteAsync(It.IsAny<Guid>()))
			.ReturnsAsync(true);

        // Act
        await _service.DeleteGenreAsync(id);

        // Assert
        _genreRepositoryMock.Verify(_ => _.DeleteAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void DeleteGenreAsync_UnexistingGenreId_ThrowsResourceNotFoundException()
    {
        // Arrange 
        var id = Guid.NewGuid();

        _genreRepositoryMock.Setup(_ => _.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        // Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteGenreAsync(id));
    }

    [Test]
    public async Task FilterAsync_GenreRepository_ReadAllAsync_WasCalled_Once()
    {
        // Arrange
        var filters = new GenreFiltersModel();

        _genreRepositoryMock.Setup(_ => _.ReadAllAsync(It.IsAny<GenreFiltersModel>()))
            .Returns(Array.Empty<GenreModel>().ToAsyncEnumerable());

        // Act
        await _service.FilterAsync(filters).AnyAsync();

        // Assert
        _genreRepositoryMock.Verify(_ => _.ReadAllAsync(It.IsAny<GenreFiltersModel>()), Times.Once);
    }

    [TearDown]
	public void ClearMocksInvocations()
	{
		_genreRepositoryMock.Invocations.Clear();
	}
}
