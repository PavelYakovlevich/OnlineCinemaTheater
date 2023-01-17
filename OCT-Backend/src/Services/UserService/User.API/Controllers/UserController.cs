using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.UserService.Requests;
using Models.UserService.Responses;
using User.Contract.Services;
using User.Domain.Models;

namespace User.API.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] ChangeUserRequest changeUserRequest)
    {
        var userModel = _mapper.Map<ChangeUserRequest, UserModel>(changeUserRequest);
        
        await _userService.UpdateUserAsync(id, userModel);

        return NoContent();
    }

    [HttpPost("{id}/photo")]
    public async Task<IActionResult> UploadUserPhoto([FromRoute] Guid id, [FromForm] IFormFile photo)
    {
        await _userService.UploadUserPhoto(id, photo);

        return NoContent();
    }

    [HttpDelete("{id}/photo")]
    public async Task<IActionResult> DeleteUserPhoto([FromRoute] Guid id)
    {
        await _userService.DeleteUserPhotoAsync(id);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserReponse>> GetUserById([FromRoute] Guid id)
    {
        var userModel = await _userService.GetUserByIdAsync(id);

        var response = _mapper.Map<GetUserReponse>(userModel);

        return Ok(response);
    }

    [HttpGet("{id}/photo")]
    [AllowAnonymous]
    public async Task<ActionResult<GetUserReponse>> GetUserPhoto([FromRoute] Guid id)
    {
        var photo = await _userService.GetUserPhotoAsync(id);

        return File(photo, "image/jpeg");
    }
}
