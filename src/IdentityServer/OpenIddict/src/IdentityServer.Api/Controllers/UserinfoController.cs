using System.Security.Claims;
using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Api.Controllers;

[ApiController]
public class UserinfoController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserinfoController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// OpenID Connect Userinfo endpoint - returns user claims based on the access token
    /// </summary>
    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("~/connect/userinfo")]
    [HttpPost("~/connect/userinfo")]
    [Produces("application/json")]
    public async Task<IActionResult> Userinfo()
    {
        // Get the user from the access token
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified access token is invalid."
                }));
        }

        // Build the claims dictionary based on requested scopes
        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            // The "sub" (subject) claim is always included
            [Claims.Subject] = user.Id.ToString()
        };

        // Add claims based on the scopes granted to the access token
        if (User.HasScope(Scopes.Profile))
        {
            claims[Claims.Name] = user.UserName ?? string.Empty;
            claims[Claims.GivenName] = user.FirstName ?? string.Empty;
            claims[Claims.FamilyName] = user.LastName ?? string.Empty;
            claims[Claims.PreferredUsername] = user.UserName ?? string.Empty;

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                claims[Claims.Picture] = user.ProfilePictureUrl;
            }
        }

        if (User.HasScope(Scopes.Email))
        {
            claims[Claims.Email] = user.Email ?? string.Empty;
            claims[Claims.EmailVerified] = user.EmailConfirmed;
        }

        if (User.HasScope(Scopes.Phone))
        {
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                claims[Claims.PhoneNumber] = user.PhoneNumber;
                claims[Claims.PhoneNumberVerified] = user.PhoneNumberConfirmed;
            }
        }

        if (User.HasScope(Scopes.Roles))
        {
            var roles = await _userManager.GetRolesAsync(user);
            claims[Claims.Role] = roles.ToArray();
        }

        // Add custom claims
        claims["created_at"] = new DateTimeOffset(user.CreatedAt).ToUnixTimeSeconds();

        if (user.LastLoginAt.HasValue)
        {
            claims["last_login_at"] = new DateTimeOffset(user.LastLoginAt.Value).ToUnixTimeSeconds();
        }

        claims["is_active"] = user.IsActive;

        // Return the claims as a JSON object
        return Ok(claims);
    }
}
