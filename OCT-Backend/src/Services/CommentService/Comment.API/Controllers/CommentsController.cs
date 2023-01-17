using AutoMapper;
using Comment.Contract.Services;
using Comment.Domain;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.CommentService.Requests;
using Models.CommentService.Responses;
using System.Security.Claims;

namespace Comment.API.Controllers;

[Route("api/medias")]
[ApiController]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _service;
    private readonly IMapper _mapper;

    public CommentsController(ICommentService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost("{mediaId}/comments")]
    public async Task<IActionResult> CreateComment(Guid mediaId, CreateCommentRequest request)
    {
        var commentModel = _mapper.Map<CommentModel>(request);
        commentModel.MediaId = mediaId;

        var id = await _service.CreateAsync(commentModel);
        
        return Ok(id);
    }

    [HttpGet("{mediaId}/comments")]
    [AllowAnonymous]
    public IActionResult GetComments([FromRoute] Guid mediaId, [FromQuery] GetCommentsFilters filters)
    {
        var filtersModel = _mapper.Map<CommentsFilters>(filters);

        var comments = _service.GetCommentsAsync(mediaId, filtersModel)
            .Select(c => _mapper.Map<GetCommentResponse>(c));

        return Ok(comments);
    }

    [HttpDelete("{mediaId}/comments/{id}")]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid id)
    {
        if (!await UserHasAccess(id))
        {
            return Forbid();
        }

        await _service.DeleteAsync(id);

        return NoContent();
    }

    [HttpPut("{mediaId}/comments/{id}")]
    [Authorize("UserOnly")]
    public async Task<IActionResult> UpdateComment(Guid mediaId, Guid id, [FromBody] UpdateCommentRequest request)
    {
        if (!await UserHasAccess(id))
        {
            return Forbid();
        }

        var commentModel = _mapper.Map<CommentModel>(request);
        commentModel.MediaId = mediaId;

        await _service.UpdateAsync(id, commentModel);

        return NoContent();
    }

    private async Task<bool> UserHasAccess(Guid id)
    {
        if (HttpContext.User.IsInRole("Moderator"))
        {
            return true;
        }

        var comment = await _service.GetByIdAsync(id);

        var userIdClaimValue = new Guid(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        return userIdClaimValue == comment.UserId;
    }
}
