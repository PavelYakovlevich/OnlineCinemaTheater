using AutoMapper;
using MediaInfo.Contract.Services;
using MediaInfo.Domain.Genre;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.MediaInfoService.Request.Genre;
using Models.MediaInfoService.Response.Genre;

namespace MediaInfo.API.Controllers;

[ApiController]
[Route("api/genres")]
[Authorize("ModeratorOnly")]
public class GenreController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IGenreService _service;

    public GenreController(IMapper mapper, IGenreService service)
    {
        _mapper = mapper;
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenre(GenreRequest request)
    {
        var genreModel = _mapper.Map<GenreModel>(request);

        var id = await _service.CreateGenreAsync(genreModel);

        return CreatedAtAction(nameof(CreateGenre), id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        await _service.DeleteGenreAsync(id);

        return NoContent();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult FilterGenres([FromQuery] GenresRequestFilters request)
    {
        var filtersModel = _mapper.Map<GenreFiltersModel>(request);

        var genres = _service.FilterAsync(filtersModel)
            .Select(gm => _mapper.Map<GenreResponse>(gm));

        return Ok(genres);
    }
}
