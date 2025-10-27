using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Api.Controllers;

[ApiController]
public class LogoutController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutController(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    /// <summary>
    /// OpenID Connect Logout endpoint - handles logout requests
    /// </summary>
    [HttpGet("~/connect/logout")]
    [HttpPost("~/connect/logout")]
    public async Task<IActionResult> Logout()
    {
        // Ask OpenIddict to delete the logout request from the distributed cache
        var request = HttpContext.GetOpenIddictServerRequest();

        // Sign out from the identity provider
        await _signInManager.SignOutAsync();

        // If this is an end session request, return a sign-out response
        if (request != null)
        {
            // Returning a SignOutResult will ask OpenIddict to redirect the user agent
            // to the post_logout_redirect_uri specified by the client application or to
            // the RedirectUri specified in the authentication properties if none was set.
            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
        }

        // If there's no request, just sign out
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// OpenID Connect End Session endpoint callback
    /// </summary>
    [HttpGet("~/connect/logout/callback")]
    public IActionResult LogoutCallback()
    {
        // This endpoint is used by OpenIddict to perform the final step of the logout flow.
        // You can customize the response based on your requirements.
        return Ok(new { message = "You have been successfully logged out." });
    }
}
