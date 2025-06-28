using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Identity.Commands;
using ProductManager.Domain.Common;
using ProductManager.Shared.DTOs.UserDto;
using static Microsoft.AspNetCore.Http.StatusCodes;
namespace ProductManager.Api.Controllers;

/// <summary>
///     Identity controller for user authentication and registration
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/identity")]
public class IdentityController : ControllerBase
{
    private readonly Dispatcher _dispatcher;
    public IdentityController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(Status204NoContent)]
    [ProducesResponseType(Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse>> Login(LoginRequest parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse(Status400BadRequest, "InvalidData"));
        }

        var result = await _dispatcher.DispatchAsync(new UserLoginCommand(parameters));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse>> RefreshToken([FromBody] RefreshTokenDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse(Status400BadRequest, "InvalidData"));
        }

        var result = await _dispatcher.DispatchAsync(new UserRefreshTokenCommand(request.AccessToken, request.RefreshToken));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Register(RegisterRequest parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse(Status400BadRequest, "InvalidData"));
        }

        var result = await _dispatcher.DispatchAsync(new UserCreateCommand(parameters));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ConfirmEmail(ConfirmEmailDto parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse(Status400BadRequest, "InvalidData"));
        }

        var result = await _dispatcher.DispatchAsync(new UserConfirmEmailCommand(parameters));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("logout")]
    [ProducesResponseType(Status204NoContent)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Logout()
    {
        var result = await _dispatcher.DispatchAsync(new UserLogOutCommand(User));
        return StatusCode(result.StatusCode, result);
    }
}
