using System.Collections.Immutable;
using System.Security.Claims;
using IdentityServer.Domain.Contracts;
using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Api.Controllers;

[ApiController]
public class TokenController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly ISessionService _sessionService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly ILogger<TokenController> _logger;

    public TokenController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager,
        ISessionService sessionService,
        ITwoFactorService twoFactorService,
        ILogger<TokenController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
        _sessionService = sessionService;
        _twoFactorService = twoFactorService;
        _logger = logger;
    }

    [HttpPost("~/connect/token")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsPasswordGrantType())
        {
            return await HandlePasswordGrantAsync(request);
        }

        if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            return await HandleAuthorizationCodeOrRefreshTokenGrantAsync();
        }

        if (request.IsClientCredentialsGrantType())
        {
            return await HandleClientCredentialsGrantAsync(request);
        }

        throw new NotImplementedException("The specified grant type is not implemented.");
    }

    private async Task<IActionResult> HandlePasswordGrantAsync(OpenIddictRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username!)
            ?? await _userManager.FindByEmailAsync(request.Username!);

        if (user == null)
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                }));
        }

        // Validate the password
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password!, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                }));
        }

        // Check if 2FA is enabled for this user
        if (await _twoFactorService.IsEnabledAsync(user.Id))
        {
            // Check if 2FA code is provided
            var twoFactorCode = request.GetParameter("2fa_code")?.ToString();
            if (string.IsNullOrEmpty(twoFactorCode))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Two-factor authentication code is required."
                    }));
            }

            // Verify 2FA code
            var isValidCode = await _twoFactorService.VerifyCodeAsync(user.Id, twoFactorCode);
            if (!isValidCode)
            {
                // Try backup code if TOTP failed
                isValidCode = await _twoFactorService.VerifyBackupCodeAsync(user.Id, twoFactorCode);
            }

            if (!isValidCode)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid two-factor authentication code."
                    }));
            }
        }

        // Create the claims-based identity
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Subject, user.Id.ToString())
                .SetClaim(Claims.Email, user.Email)
                .SetClaim(Claims.Name, user.UserName)
                .SetClaim(Claims.GivenName, user.FirstName)
                .SetClaim(Claims.FamilyName, user.LastName)
                .SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

        identity.SetDestinations(GetDestinations);

        // Create session tracking (on password grant with refresh token)
        if (request.GetScopes().Contains("offline_access"))
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                // We'll store the session ID in a claim to track it
                var sessionId = await _sessionService.CreateSessionAsync(
                    user.Id,
                    refreshToken: null, // Will be set when refresh token is generated
                    ipAddress,
                    userAgent);

                identity.SetClaim("session_id", sessionId.ToString());

                _logger.LogInformation("Created session {SessionId} for user {UserId}", sessionId, user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create session for user {UserId}", user.Id);
                // Continue anyway - session tracking is not critical for authentication
            }
        }

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandleAuthorizationCodeOrRefreshTokenGrantAsync()
    {
        var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal
            ?? throw new InvalidOperationException("The claims principal cannot be retrieved.");

        var user = await _userManager.FindByIdAsync(claimsPrincipal.GetClaim(Claims.Subject)!);
        if (user == null)
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                }));
        }

        // Ensure the user is still allowed to sign in
        if (!await _signInManager.CanSignInAsync(user))
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                }));
        }

        var identity = new ClaimsIdentity(claimsPrincipal.Claims,
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Subject, user.Id.ToString())
                .SetClaim(Claims.Email, user.Email)
                .SetClaim(Claims.Name, user.UserName)
                .SetClaim(Claims.GivenName, user.FirstName)
                .SetClaim(Claims.FamilyName, user.LastName)
                .SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandleClientCredentialsGrantAsync(OpenIddictRequest request)
    {
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
            ?? throw new InvalidOperationException("The application cannot be found.");

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application))
                .SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Add claim to access_token
        yield return Destinations.AccessToken;

        // Add specific claims to id_token
        if (claim.Type is Claims.Name or Claims.Email or Claims.Subject or Claims.GivenName or Claims.FamilyName)
        {
            yield return Destinations.IdentityToken;
        }
    }
}
