using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Feature.Identity.Commands;
using ProductManager.Domain.Common;
using ProductManager.Shared.DTOs.UserDto;
using Dispatcher=ProductManager.Application.Common.Dispatcher;
namespace ProductManager.Api.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
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
    public async Task<ApiResponse> Login(LoginRequest parameters) => ModelState.IsValid
        ? await _dispatcher.DispatchAsync(new UserLoginCommand(parameters))
        : new ApiResponse(Status400BadRequest, "InvalidData");

    [HttpPost("refresh-token")]
    public async Task<ApiResponse> RefreshToken([FromBody] RefreshTokenDto request)
        => ModelState.IsValid
            ? await _dispatcher.DispatchAsync(new UserRefreshTokenCommand(request.AccessToken, request.RefreshToken))
            : new ApiResponse(Status400BadRequest, "InvalidData");

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<ApiResponse> Register(RegisterRequest parameters) => ModelState.IsValid
        ? await _dispatcher.DispatchAsync(new UserCreateCommand(parameters))
        : new ApiResponse(Status400BadRequest, "InvalidData");

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<ApiResponse> ConfirmEmail(ConfirmEmailDto parameters) => ModelState.IsValid
        ? await _dispatcher.DispatchAsync(new UserConfirmEmailCommand(parameters))
        : new ApiResponse(Status400BadRequest, "InvalidData");

    [HttpPost("logout")]
    [ProducesResponseType(Status204NoContent)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<ApiResponse> Logout(ClaimsPrincipal parameters) => ModelState.IsValid
        ? await _dispatcher.DispatchAsync(new UserLogOutCommand(parameters))
        : new ApiResponse(Status400BadRequest, "InvalidData");
}
