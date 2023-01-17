using Authentication.API.Extensions;
using Authentication.Contract.Services;
using Authentication.Domain.Models;
using AutoMapper;
using Enums.AuthentificationService;
using Exceptions.AuthentificationService;
using Microsoft.AspNetCore.Mvc;
using Models.AuthenticationService.Request;

namespace Authentication.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IAuthenticationService _authorizationService;
    private readonly IMapper _mapper;

    public AccountController(IAccountService service, IAuthenticationService authenticationService, IMapper mapper)
    {
        _accountService = service;
        _authorizationService = authenticationService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] AccountRequest accountModel)
    {
        var account = _mapper.Map<AccountModel>(accountModel);

        account = await _accountService.CreateAccountAsync(account, AccountRole.User);

        return Created(HttpContext.Request.Path, account.Id);
    }

    [HttpPut("{id}/password")]
    public async Task<IActionResult> UpdatePassword(Guid id, [FromBody] ChangeAccountRequest accountRequest, [FromQuery] string token)
    {
        await _accountService.UpdatePasswordAsync(id, token, accountRequest.Password);

        return NoContent();
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        await _accountService.ConfirmAccountEmailAsync(token);

        return NoContent();
    }


    [HttpPost("change-password")]
    public async Task<IActionResult> SendForgottenPasswordEmail([FromBody] SendChangePasswordEmailRequest emailModel)
    {
        await _accountService.SendChangePasswordEmailAsync(emailModel.Email);

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate(AccountRequest accountRequest)
    {
        var (accessToken, refreshToken) = await _authorizationService.AuthenticateAsync(accountRequest.Email, accountRequest.Password);

        HttpContext.Response.AddAuthTokens(accessToken, refreshToken);

        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"]
            ?? throw new RefreshTokenException($"Refresh token cookie 'refreshToken' was not defined.");

        (string accessToken, refreshToken) = await _authorizationService.RefreshAsync(refreshToken);

        HttpContext.Response.AddAuthTokens(accessToken, refreshToken);

        return Ok();
    }
}
