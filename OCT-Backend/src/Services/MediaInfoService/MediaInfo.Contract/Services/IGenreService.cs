using MediaInfo.Domain.Genre;

namespace MediaInfo.Contract.Services;

public interface IGenreService
{
    Task<Guid> CreateGenreAsync(GenreModel genreModel);

    Task DeleteGenreAsync(Guid genreId);

    IAsyncEnumerable<GenreModel> FilterAsync(GenreFiltersModel filters);
}
