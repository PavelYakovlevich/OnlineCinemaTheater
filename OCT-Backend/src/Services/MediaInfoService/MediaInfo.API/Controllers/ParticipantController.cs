using AutoMapper;
using MediaInfo.API.Extensions;
using MediaInfo.Contract.Services;
using MediaInfo.Domain.Participant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.MediaInfoService.Request.Participant;
using Models.MediaInfoService.Response.Participant;

namespace MediaInfo.API.Controllers;

[Route("api/participants")]
[ApiController]
[Authorize("ModeratorOnly")]
public class ParticipantController : ControllerBase
{
    private readonly IParticipantService _service;
    private readonly IMapper _mapper;

    public ParticipantController(IParticipantService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateParticipant(ParticipantRequest request)
    {
        var participantModel = _mapper.Map<ParticipantModel>(request);

        var participantId = await _service.CreateAsync(participantModel);

        return CreatedAtAction(nameof(CreateParticipant), participantId);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetParticipantById(Guid id)
    {
        var participantModel = await _service.GetByIdAsync(id);

        var getParticipantResponse = _mapper.Map<ParticipantResponse>(participantModel);

        return Ok(getParticipantResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParticipant(Guid id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateParticipant(Guid id, [FromBody] ParticipantRequest request)
    {
        var participantModel = _mapper.Map<ParticipantModel>(request);

        await _service.UpdateAsync(id, participantModel);

        return NoContent();
    }

    [HttpPost("{id}/photo")]
    public async Task<IActionResult> UploadParticipantPicture([FromRoute] Guid id, [FromForm] IFormFile picture)
    {
        await _service.UploadPictureAsync(id, picture);

        return NoContent();
    }

    [HttpDelete("{id}/photo")]
    public async Task<IActionResult> DeleteParticipantPicture(Guid id)
    {
        await _service.DeletePictureAsync(id);

        return NoContent();
    }

    [HttpGet("{id}/photo")]
    [AllowAnonymous]
    public async Task<IActionResult> GetParticipantPicture(Guid id)
    {
        var pictureStream = await _service.GetPictureAsync(id);

        return File(pictureStream, "image/jpeg");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetParticipants([FromQuery] ParticipantsRequestFilters filters)
    {
        var filtersModel = _mapper.Map<ParticipantFiltersModel>(filters);

        var participants = _service.FilterAsync(filtersModel);

        return Ok(_mapper.Map<ParticipantModel, ParticipantResponse>(participants));
    }
}
