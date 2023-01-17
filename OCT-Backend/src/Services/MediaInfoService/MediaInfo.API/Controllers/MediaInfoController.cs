using AutoMapper;
using FluentValidation;
using MediaInfo.API.Extensions;
using MediaInfo.API.Validators.MediaInfo;
using MediaInfo.Contract.Services;
using MediaInfo.Domain.MediaInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Models.MediaInfoService.Request.MediaInfo;
using Models.MediaInfoService.Response.MediaInfo;

namespace MediaInfo.API.Controllers;

[ApiController]
[Route("api/media-infos")]
[Authorize("ModeratorOnly")]
public class MediaInfoController : ControllerBase
{
    private readonly IMediaInfoService _service;
    private readonly IMapper _mapper;
    private readonly PartialUpdateMediaInfoValidator _partialUpdateMediaInfoValidator;

    public MediaInfoController(IMediaInfoService service, IMapper mapper, PartialUpdateMediaInfoValidator partialUpdateMediaInfoValidator)
    {
        _service = service;
        _mapper = mapper;
        _partialUpdateMediaInfoValidator = partialUpdateMediaInfoValidator;
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMediaById(Guid id)
    {
        var model = await _service.GetByIdAsync(id);

        var response = _mapper.Map<GetMediaResponse>(model);

        return Ok(response);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAllWithFilters([FromQuery] MediaInfoRequestFilters filters)
    {
        var filtersModel = _mapper.Map<MediaInfoFiltersModel>(filters);

        var models = _service.GetAllAsync(filtersModel);

        var response = _mapper.Map<MediaInfoModel, GetMediaResponse>(models);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedia(Guid id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateMedia(Guid id, [FromBody] JsonPatchDocument<PartialUpdateMediaInfoModel> patchDocument)
    {
        var partialUpdateModel = await GetPartialUpdateModel(id, patchDocument);

        await _service.UpdateAsync(id, partialUpdateModel);

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMedia(MediaInfoRequest request)
    {
        var model = _mapper.Map<MediaInfoModel>(request);

        var id = await _service.CreateAsync(model);

        return CreatedAtAction(nameof(CreateMedia), id);
    }

    [HttpGet("{id}/picture")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMediaPhoto(Guid id)
    {
        var pictureStream = await _service.GetPictureAsync(id);

        return File(pictureStream, "image/jpeg");
    }

    [HttpPost("{id}/picture")]
    public async Task<IActionResult> UploadMediaPhoto(Guid id, IFormFile picture)
    {
        await _service.UploadPictureAsync(id, picture);

        return NoContent();
    }

    [HttpDelete("{id}/picture")]
    public async Task<IActionResult> DeleteMediaPhoto(Guid id)
    {
        await _service.DeletePictureAsync(id);

        return NoContent();
    }

    private async Task<PartialUpdateMediaInfoModel> GetPartialUpdateModel(Guid id, JsonPatchDocument<PartialUpdateMediaInfoModel> patchDocument)
    {
        var mediaInfoModel = await _service.GetByIdAsync(id);

        var partialUpdateModel = _mapper.Map<PartialUpdateMediaInfoModel>(mediaInfoModel);

        patchDocument.ApplyTo(partialUpdateModel);

        await ValidatePartialUpdateModel(partialUpdateModel);

        return partialUpdateModel;
    }

    private async Task ValidatePartialUpdateModel(PartialUpdateMediaInfoModel partialUpdateModel) =>
        await _partialUpdateMediaInfoValidator.ValidateAndThrowAsync(partialUpdateModel);
}
