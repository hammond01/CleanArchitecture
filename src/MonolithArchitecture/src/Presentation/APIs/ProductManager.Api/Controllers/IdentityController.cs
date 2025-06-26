using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Identity.Commands;
using ProductManager.Domain.Common;
using ProductManager.Shared.DTOs.UserDto;
using static Microsoft.AspNetCore.Http.StatusCodes;
namespace ProductManager.Api.Controllers;

/// <summary>
/// Identity controller for user authentication and registration
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")] // Fallback route for backward compatibility
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
    public async Task<IActionResult> Login(LoginRequest parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse(Status400BadRequest, "InvalidData"));
        }

        var result = await _dispatcher.DispatchAsync(new UserLoginCommand(parameters));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
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
    public async Task<IActionResult> Register(RegisterRequest parameters)
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
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto parameters)
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
    public async Task<IActionResult> Logout()
    {
        var result = await _dispatcher.DispatchAsync(new UserLogOutCommand(User));
        return StatusCode(result.StatusCode, result);
    }
}
