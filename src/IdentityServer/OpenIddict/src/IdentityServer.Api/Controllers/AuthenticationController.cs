using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthenticationController> logger,
        IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Simple login endpoint that internally calls OpenIddict token endpoint
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
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

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Call OpenIddict token endpoint internally
        var client = _httpClientFactory.CreateClient();
        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["username"] = request.Email,
            ["password"] = request.Password,
            ["scope"] = "openid profile email roles"
        };

        var tokenResponse = await client.PostAsync(
            "http://localhost:5000/connect/token",
            new FormUrlEncodedContent(tokenRequest));

        if (!tokenResponse.IsSuccessStatusCode)
        {
            return StatusCode((int)tokenResponse.StatusCode, new { message = "Failed to generate token" });
        }

        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("User {Email} logged in successfully", user.Email);

        return Ok(new
        {
            access_token = tokenData?.AccessToken,
            token_type = "Bearer",
            expires_in = tokenData?.ExpiresIn ?? 3600,
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
}

public class JwtLoginRequest
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
