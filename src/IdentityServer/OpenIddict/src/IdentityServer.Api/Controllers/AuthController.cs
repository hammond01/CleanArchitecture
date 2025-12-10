using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Application.Commands;
using IdentityServer.Domain.Common;
using IdentityServer.Domain.Contracts;

namespace IdentityServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>User ID if successful</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        var command = new RegisterUserCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("User registered successfully with ID: {UserId}", result.Data);
            return Ok(new RegisterResponse
            {
                UserId = result.Data,
                Message = "User registered successfully"
            });
        }

        _logger.LogWarning("Registration failed for {Email}: {Error}", request.Email, result.ErrorMessage);
        return BadRequest(new ErrorResponse
        {
            Error = result.ErrorMessage ?? "",
            Errors = result.Errors
        });
    }

    /// <summary>
    /// Login with username and password - Returns OpenIddict token endpoint URL
    /// For actual token, POST to /connect/token with grant_type=password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Token endpoint information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for: {UserNameOrEmail}", request.UserNameOrEmail);

        var command = new LoginUserCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Credentials validated successfully for: {UserNameOrEmail}", request.UserNameOrEmail);

            // Return instructions to get token from OpenIddict endpoint
            return Ok(new LoginResponse
            {
                Message = "Credentials validated. Use /connect/token endpoint to obtain access token.",
                TokenEndpoint = $"{Request.Scheme}://{Request.Host}/connect/token",
                GrantType = "password",
                Username = request.UserNameOrEmail,
                RequiresTokenExchange = true
            });
        }

        _logger.LogWarning("Login failed for {UserNameOrEmail}: {Error}", request.UserNameOrEmail, result.ErrorMessage);
        return Unauthorized(new ErrorResponse
        {
            Error = result.ErrorMessage ?? string.Empty,
            Errors = result.Errors
        });
    }

    /// <summary>
    /// Get current user information (requires authentication)
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
        var email = User.FindFirst("email")?.Value;
        var userName = User.Identity?.Name;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        return Ok(new UserInfoResponse
        {
            UserId = userId,
            Email = email,
            UserName = userName
        });
    }

    /// <summary>
    /// Send email confirmation link
    /// </summary>
    /// <param name="request">Email confirmation request</param>
    /// <returns>Success message</returns>
    [HttpPost("send-confirmation-email")]
    [Authorize]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendConfirmationEmail([FromBody] SendConfirmationEmailRequest request)
    {
        _logger.LogInformation("Email confirmation request for user: {UserId}", request.UserId);

        var command = new SendConfirmationEmailCommand
        {
            UserId = request.UserId,
            Email = request.Email,
            FirstName = request.FirstName
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Confirmation email sent successfully to {Email}", request.Email);
            return Ok(new MessageResponse { Message = result.Data! });
        }

        _logger.LogWarning("Failed to send confirmation email to {Email}: {Error}", request.Email, result.ErrorMessage);
        return BadRequest(new ErrorResponse { Error = (result.ErrorMessage ?? "Failed to send confirmation email")! });
    }

    /// <summary>
    /// Confirm email address
    /// </summary>
    /// <param name="request">Email confirmation details</param>
    /// <returns>Success message</returns>
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        _logger.LogInformation("Email confirmation attempt for user: {UserId}", request.UserId);

        var command = new ConfirmEmailCommand
        {
            UserId = request.UserId,
            Token = request.Token
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Email confirmed successfully for user: {UserId}", request.UserId);
            return Ok(new MessageResponse { Message = result.Data! });
        }

        _logger.LogWarning("Email confirmation failed for user {UserId}: {Error}", request.UserId, result.ErrorMessage);
        return BadRequest(new ErrorResponse { Error = (result.ErrorMessage ?? "Email confirmation failed")! });
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    /// <param name="request">Password reset request</param>
    /// <returns>Success message</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        _logger.LogInformation("Password reset request for email: {Email}", request.Email);

        var command = new ForgotPasswordCommand
        {
            Email = request.Email
        };

        var result = await _mediator.Send(command);

        // Always return success for security reasons (don't reveal if email exists)
        _logger.LogInformation("Password reset processed for {Email}", request.Email);
        return Ok(new MessageResponse { Message = result.Data! });
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    /// <param name="request">Password reset details</param>
    /// <returns>Success message</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        _logger.LogInformation("Password reset attempt for user: {UserId}", request.UserId);

        var command = new ResetPasswordCommand
        {
            UserId = request.UserId,
            Token = request.Token,
            NewPassword = request.NewPassword
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Password reset successfully for user: {UserId}", request.UserId);
            return Ok(new MessageResponse { Message = result.Data! });
        }

        _logger.LogWarning("Password reset failed for user {UserId}: {Error}", request.UserId, result.ErrorMessage);
        return BadRequest(new ErrorResponse { Error = (result.ErrorMessage ?? "Password reset failed")! });
    }
}

// Request DTOs
public class SendConfirmationEmailRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
}

public class ConfirmEmailRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

// Response DTOs
public class RegisterResponse
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Message { get; set; } = string.Empty;
    public string? TokenEndpoint { get; set; }
    public string? GrantType { get; set; }
    public string? Username { get; set; }
    public bool RequiresTokenExchange { get; set; }
}

public class MessageResponse
{
    public string Message { get; set; } = string.Empty;
}

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}

public class UserInfoResponse
{
    public string UserId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? UserName { get; set; }
}
