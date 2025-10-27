using System.Collections.Immutable;
using System.Security.Claims;
using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Api.Controllers;

[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorizationController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest() 
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Try to retrieve the user principal stored in the authentication cookie
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

        // If the user is not authenticated, redirect them to the login page
        if (!result.Succeeded)
        {
            // If the client requested prompt=none, return an error since the user is not logged in
            if (request.Prompt != null && request.Prompt.Contains("none"))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                    }));
            }

            // Redirect to login page with return URL
            var returnUrl = Request.PathBase + Request.Path + QueryString.Create(
                Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList());

            return Challenge(
                authenticationSchemes: IdentityConstants.ApplicationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = returnUrl
                });
        }

        // Retrieve the profile of the logged in user
        var user = await _userManager.GetUserAsync(result.Principal) 
            ?? throw new InvalidOperationException("The user details cannot be retrieved.");

        // Retrieve the application details from the database
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!) 
            ?? throw new InvalidOperationException("The application details cannot be found in the database.");

        // Retrieve the permanent authorizations associated with the user and the calling client application
        var authorizationsEnumerable = _authorizationManager.FindAsync(
            subject: await _userManager.GetUserIdAsync(user),
            client: (await _applicationManager.GetIdAsync(application))!,
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: request.GetScopes());
        
        var authorizations = new List<object>();
        await foreach (var auth in authorizationsEnumerable)
        {
            authorizations.Add(auth);
        }

        switch (await _applicationManager.GetConsentTypeAsync(application))
        {
            // If the consent is external (e.g., when authorizations are granted by a sysadmin),
            // immediately return an error if no authorization can be found in the database
            case ConsentTypes.External when !authorizations.Any():
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The logged in user is not allowed to access this client application."
                    }));

            // If the consent is implicit or if an authorization was found,
            // return an authorization response without displaying the consent form
            case ConsentTypes.Implicit:
            case ConsentTypes.External when authorizations.Any():
            case ConsentTypes.Explicit when authorizations.Any() && (request.Prompt == null || !request.Prompt.Contains("consent")):
                // Create the claims-based identity that will be used by OpenIddict to generate tokens
                var identity = new ClaimsIdentity(
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

                // Add the claims that will be persisted in the tokens
                identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                        .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                        .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                        .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                        .SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

                // Note: in this sample, the granted scopes match the requested scope
                // but you may want to allow the user to uncheck specific scopes.
                // For that, simply restrict the list of scopes before calling SetScopes
                identity.SetScopes(request.GetScopes());
                
                var resources = new List<string>();
                await foreach (var resource in _scopeManager.ListResourcesAsync(identity.GetScopes()))
                {
                    resources.Add(resource);
                }
                identity.SetResources(resources);

                // Automatically create a permanent authorization to avoid requiring explicit consent
                // for future authorization or token requests containing the same scopes
                var authorization = authorizations.LastOrDefault();
                var clientId = (await _applicationManager.GetIdAsync(application))!;
                authorization ??= await _authorizationManager.CreateAsync(
                    identity: identity,
                    subject: await _userManager.GetUserIdAsync(user),
                    client: clientId,
                    type: AuthorizationTypes.Permanent,
                    scopes: identity.GetScopes());

                identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
                identity.SetDestinations(GetDestinations);

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // At this point, no authorization was found in the database and an error must be returned
            // if the client application specified prompt=none in the authorization request
            case ConsentTypes.Explicit when request.Prompt != null && request.Prompt.Contains("none"):
            case ConsentTypes.Systematic when request.Prompt != null && request.Prompt.Contains("none"):
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Interactive user consent is required."
                    }));

            // In every other case, render the consent form
            default:
                // TODO: Implement consent page rendering
                // For now, we'll auto-approve all requests
                return await AcceptConsent(request, user, application);
        }
    }

    [Authorize]
    [HttpPost("~/connect/authorize/accept")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Accept()
    {
        var request = HttpContext.GetOpenIddictServerRequest() 
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var user = await _userManager.GetUserAsync(User) 
            ?? throw new InvalidOperationException("The user details cannot be retrieved.");

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!) 
            ?? throw new InvalidOperationException("The application details cannot be found.");

        return await AcceptConsent(request, user, application);
    }

    [Authorize]
    [HttpPost("~/connect/authorize/deny")]
    [IgnoreAntiforgeryToken]
    public IActionResult Deny()
    {
        // Notify OpenIddict that the authorization grant has been denied by the resource owner
        return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> AcceptConsent(
        OpenIddictRequest request,
        ApplicationUser user,
        object application)
    {
        // Create the claims-based identity that will be used by OpenIddict to generate tokens
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Add the claims that will be persisted in the tokens
        identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

        // Note: in this sample, the granted scopes match the requested scope
        identity.SetScopes(request.GetScopes());
        
        var resources = new List<string>();
        await foreach (var resource in _scopeManager.ListResourcesAsync(identity.GetScopes()))
        {
            resources.Add(resource);
        }
        identity.SetResources(resources);

        // Automatically create a permanent authorization to avoid requiring explicit consent
        // for future authorization or token requests containing the same scopes
        var clientId = (await _applicationManager.GetIdAsync(application))!;
        var authorization = await _authorizationManager.CreateAsync(
            identity: identity,
            subject: await _userManager.GetUserIdAsync(user),
            client: clientId,
            type: AuthorizationTypes.Permanent,
            scopes: identity.GetScopes());

        identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
        identity.SetDestinations(GetDestinations);

        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens
        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case Claims.Name or Claims.PreferredUsername:
                yield return Destinations.AccessToken;

                if (claim.Subject?.HasScope(Scopes.Profile) == true)
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (claim.Subject?.HasScope(Scopes.Email) == true)
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (claim.Subject?.HasScope(Scopes.Roles) == true)
                    yield return Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value
            case "AspNet.Identity.SecurityStamp":
                yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}
