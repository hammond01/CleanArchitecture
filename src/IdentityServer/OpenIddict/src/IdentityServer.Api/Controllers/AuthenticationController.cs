using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace IdentityServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthenticationController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint - Use OpenIddict's /connect/token endpoint directly for production
    /// This endpoint is for demonstration/migration purposes only
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [Obsolete("Use /connect/token with grant_type=password instead")]
    public async Task<IActionResult> Login([FromBody] JwtLoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return Unauthorized(new { message = "Account locked. Please try again later." });

            return Unauthorized(new { message = "Invalid email or password" });
        }

        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("User {Email} validated. Redirect to /connect/token for actual token", user.Email);

        // Return instructions to use proper OpenIddict endpoint
        return Ok(new
        {
            message = "Credentials validated. Please use /connect/token endpoint for token generation.",
            token_endpoint = $"{Request.Scheme}://{Request.Host}/connect/token",
            grant_type = "password",
            username = request.Email,
            recommended_scopes = "openid profile email roles",
            user = new
            {
                id = user.Id,
                email = user.Email,
                userName = user.UserName,
                firstName = user.FirstName,
                lastName = user.LastName,
                roles = roles
            }
        });
    }
}public class JwtLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class TokenResponse
{
    public string? AccessToken { get; set; }
    public string? TokenType { get; set; }
    public int ExpiresIn { get; set; }
}
