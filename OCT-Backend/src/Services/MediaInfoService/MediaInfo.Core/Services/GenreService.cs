using Exceptions;
using MediaInfo.Contract.Repositories;
using MediaInfo.Contract.Services;
using MediaInfo.Domain.Genre;
using Serilog;

namespace MediaInfo.Core.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _repository;

    public GenreService(IGenreRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateGenreAsync(GenreModel genreModel)
    {
        var id = await _repository.CreateAsync(genreModel);

        Log.Debug("Genre with id {id} was created", id);

        return id;
    }

    public async Task DeleteGenreAsync(Guid genreId)
    {
        if (!await _repository.DeleteAsync(genreId))
        {
            throw new ResourceNotFoundException($"Genre with id {genreId} was not found");
        }

        Log.Debug("Genre with id {id} was deleted", genreId);
    }

    public async IAsyncEnumerable<GenreModel> FilterAsync(GenreFiltersModel filters)
    {
        var genres = _repository.ReadAllAsync(filters);

        Log.Debug("{count} participants were found with filters {@filter}", await genres.CountAsync(), filters);

        await foreach (var genre in genres)
        {
            yield return genre;
        }
    }
}
