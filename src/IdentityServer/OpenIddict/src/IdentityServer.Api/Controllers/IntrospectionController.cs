using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Api.Controllers;

[ApiController]
public class IntrospectionController : ControllerBase
{
    private readonly IOpenIddictTokenManager _tokenManager;
    private readonly IOpenIddictApplicationManager _applicationManager;

    public IntrospectionController(
        IOpenIddictTokenManager tokenManager,
        IOpenIddictApplicationManager applicationManager)
    {
        _tokenManager = tokenManager;
        _applicationManager = applicationManager;
    }

    /// <summary>
    /// OAuth2 Token Introspection endpoint - validates tokens and returns their metadata
    /// Used by resource servers to validate access tokens
    /// </summary>
    [HttpPost("~/connect/introspect")]
    [Produces("application/json")]
    public async Task<IActionResult> Introspect()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the token from the request
        var token = request.Token;
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new
            {
                error = Errors.InvalidRequest,
                error_description = "The token parameter is missing."
            });
        }

        // Authenticate the client making the introspection request
        var clientId = request.ClientId;
        if (string.IsNullOrEmpty(clientId))
        {
            return BadRequest(new
            {
                error = Errors.InvalidClient,
                error_description = "The client credentials are missing."
            });
        }

        var application = await _applicationManager.FindByClientIdAsync(clientId);
        if (application == null)
        {
            return BadRequest(new
            {
                error = Errors.InvalidClient,
                error_description = "The specified client identifier is invalid."
            });
        }

        // Try to find the token in the database
        var tokenEntry = await _tokenManager.FindByIdAsync(token);
        if (tokenEntry == null)
        {
            // Try to find by reference token
            tokenEntry = await _tokenManager.FindByReferenceIdAsync(token);
        }

        // If the token doesn't exist or is not active, return inactive
        if (tokenEntry == null)
        {
            return Ok(new { active = false });
        }

        // Check if the token is still valid
        var status = await _tokenManager.GetStatusAsync(tokenEntry);
        if (status != Statuses.Valid)
        {
            return Ok(new { active = false });
        }

        // Get token details
        var subject = await _tokenManager.GetSubjectAsync(tokenEntry);
        var expirationDate = await _tokenManager.GetExpirationDateAsync(tokenEntry);
        var type = await _tokenManager.GetTypeAsync(tokenEntry);

        // Return token metadata
        var response = new Dictionary<string, object>
        {
            ["active"] = true,
            ["token_type"] = type ?? "access_token",
            ["sub"] = subject ?? string.Empty,
            ["client_id"] = clientId
        };

        if (expirationDate.HasValue)
        {
            response["exp"] = expirationDate.Value.ToUnixTimeSeconds();
        }

        return Ok(response);
    }
}
