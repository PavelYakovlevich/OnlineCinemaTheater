using AutoMapper;
using Media.Contract.Services;
using Media.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.MediaService.Responses;

namespace Media.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("ModeratorOnly")]
public class MediasController : ControllerBase
{
    private readonly IMediaContentService _mediaContentService;
    private readonly ITrailerService _trailerService;
    private readonly IMapper _mapper;

    public MediasController(IMediaContentService mediaContentService, ITrailerService trailerService, IMapper mapper)
    {
        _mediaContentService = mediaContentService;
        _trailerService = trailerService;
        _mapper = mapper;
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> UploadFilm([FromRoute] Guid id, [FromForm] IFormFile mediaFile)
    {
        var mediaContentModel = new MediaContentModel
        {
            MediaId = id,
            ContentFile = mediaFile
        };

        var mediaContentId = await _mediaContentService.CreateMediaContentAsync(mediaContentModel);

        return Ok(mediaContentId);
    }

    [HttpPost("{mediaId}/serias/{number}")]
    public async Task<IActionResult> UploadSeria(Guid mediaId, int number, [FromForm] IFormFile contentFile)
    {
        var mediaContentModel = new MediaContentModel
        {
            MediaId = mediaId,
            Number = number,
            ContentFile = contentFile
        };

        var mediaContentId = await _mediaContentService.CreateMediaContentAsync(mediaContentModel);

        return Ok(mediaContentId);
    }

    [HttpGet("{mediaId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFilm(Guid mediaId)
    {
        var file = await _mediaContentService.GetMediaContentFileAsync(mediaId);

        return File(file, "video/mp4");
    }

    [HttpGet("{mediaId}/serias/{number}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSeria(Guid mediaId, [FromRoute] int number)
    {
        var file = await _mediaContentService.GetMediaContentFileAsync(mediaId, number);

        return File(file, "video/mp4");
    }

    [HttpDelete("{mediaId}")]
    public async Task<IActionResult> DeleteFilm(Guid mediaId)
    {
        await _mediaContentService.DeleteMediaContentAsync(mediaId);

        return NoContent();
    }

    [HttpDelete("{mediaId}/serias/{number}")]
    public async Task<IActionResult> DeleteSeria(Guid mediaId, int number)
    {
        await _mediaContentService.DeleteMediaContentAsync(mediaId, number);

        return NoContent();
    }

    [HttpGet("{mediaId}/content")]
    [AllowAnonymous]
    public IActionResult GetAllMediaContent(Guid mediaId)
    {
        var allContent = _mediaContentService.GetMediaContentAsync(mediaId)
            .Select(m => _mapper.Map<MediaContentResponse>(m));

        return Ok(allContent);
    }

    [HttpGet("{mediaId}/trailer")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMediaTrailer(Guid mediaId)
    {
        var trailer = await _trailerService.GetTrailerAsync(mediaId);

        return File(trailer, "video/mp4");
    }

    [HttpPost("{mediaId}/trailer")]
    public async Task<IActionResult> UploadMediaTrailer(Guid mediaId, [FromForm] IFormFile contentFile)
    {
        await _trailerService.UploadTrailerAsync(mediaId, contentFile);

        return NoContent();
    }

    [HttpDelete("{mediaId}/trailer")]
    public async Task<IActionResult> DeleteMediaTrailer(Guid mediaId)
    {
        await _trailerService.DeleteTrailerAsync(mediaId);

        return NoContent();
    }
}
