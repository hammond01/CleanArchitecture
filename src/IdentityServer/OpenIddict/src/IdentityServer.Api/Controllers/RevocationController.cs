using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace IdentityServer.Api.Controllers;

[ApiController]
public class RevocationController : ControllerBase
{
    private readonly IOpenIddictTokenManager _tokenManager;

    public RevocationController(IOpenIddictTokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    /// <summary>
    /// OAuth2 Token Revocation endpoint - revokes access or refresh tokens
    /// </summary>
    [HttpPost("~/connect/revoke")]
    [Produces("application/json")]
    public async Task<IActionResult> Revoke()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the token from the request
        var token = request.Token;
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new
            {
                error = "invalid_request",
                error_description = "The token parameter is missing."
            });
        }

        // Try to find the token in the database
        var tokenEntry = await _tokenManager.FindByIdAsync(token);
        if (tokenEntry == null)
        {
            // Try to find by reference token
            tokenEntry = await _tokenManager.FindByReferenceIdAsync(token);
        }

        // If the token doesn't exist, return success anyway (per RFC 7009)
        if (tokenEntry == null)
        {
            return Ok();
        }

        // Revoke the token and any associated tokens (e.g., if revoking a refresh token, also revoke related access tokens)
        try
        {
            // Get the authorization ID associated with the token
            var authorizationId = await _tokenManager.GetAuthorizationIdAsync(tokenEntry);
            
            // Revoke the specific token
            await _tokenManager.TryRevokeAsync(tokenEntry);

            // If this is a refresh token, also revoke all associated access tokens
            if (authorizationId != null)
            {
                var tokens = _tokenManager.FindByAuthorizationIdAsync(authorizationId);
                await foreach (var relatedToken in tokens)
                {
                    await _tokenManager.TryRevokeAsync(relatedToken);
                }
            }

            return Ok();
        }
        catch (Exception)
        {
            // Even if revocation fails, we should return success (per RFC 7009)
            return Ok();
        }
    }
}
