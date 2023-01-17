using MediaInfo.Domain.Genre;

namespace MediaInfo.Contract.Repositories;

public interface IGenreRepository
{
    Task<Guid> CreateAsync(GenreModel genreModel);

    Task<bool> DeleteAsync(Guid id);

    IAsyncEnumerable<GenreModel> ReadAllAsync(GenreFiltersModel filtersModel);
}
