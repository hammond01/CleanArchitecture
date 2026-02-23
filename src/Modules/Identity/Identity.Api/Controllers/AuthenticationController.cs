using BuildingBlocks.Api.Controllers;
using BuildingBlocks.Application.Dispatcher;
using Identity.Application.Features.Authentication.Commands;
using Identity.Application.Features.PasswordManagement.Commands;
using Identity.Application.Features.Registration.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthenticationController : BaseController
{
    private readonly IDispatcher _dispatcher;

    public AuthenticationController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Login user with credentials
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserLoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.DispatchAsync(command, cancellationToken);
        return Ok(new { token = result });
    }

    /// <summary>
    /// Refresh JWT token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] UserRefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.DispatchAsync(command, cancellationToken);
        return Ok(new { token = result });
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] UserLogoutCommand command, CancellationToken cancellationToken)
    {
        await _dispatcher.DispatchAsync(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Register new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserCreateCommand command, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.DispatchAsync(command, cancellationToken);
        return CreatedAtAction(nameof(Register), new { userId = result });
    }

    /// <summary>
    /// Confirm user email
    /// </summary>
    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] UserConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        await _dispatcher.DispatchAsync(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("request-password-reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetCommand command, CancellationToken cancellationToken)
    {
        await _dispatcher.DispatchAsync(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Reset password
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        await _dispatcher.DispatchAsync(command, cancellationToken);
        return Ok();
    }
}
