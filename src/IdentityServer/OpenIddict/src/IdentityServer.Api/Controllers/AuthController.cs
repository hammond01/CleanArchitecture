using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Application.Commands;
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
            Error = result.ErrorMessage,
            Errors = result.Errors
        });
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication token if successful</returns>
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
            _logger.LogInformation("User logged in successfully: {UserNameOrEmail}", request.UserNameOrEmail);
            return Ok(new LoginResponse
            {
                Token = result.Data ?? string.Empty,
                Message = "Login successful"
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
}

// Response DTOs
public class RegisterResponse
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
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
