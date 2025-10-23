using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using IdentityServer.Domain.Entities;
using System.Collections.Immutable;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Web.Pages.Account;

[Authorize]
public class AuthorizeModel : PageModel
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AuthorizeModel> _logger;

    public AuthorizeModel(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthorizeModel> logger)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public string? ApplicationName { get; set; }
    public string? ClientId { get; set; }
    public string? RedirectUri { get; set; }
    public string? UserName { get; set; }
    public List<string> RequestedScopes { get; set; } = new();
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            return BadRequest("Invalid OpenIddict request");
        }

        // Retrieve the user principal stored in the authentication cookie
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (result?.Principal == null)
        {
            return Challenge(
                authenticationSchemes: IdentityConstants.ApplicationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.Path + Request.QueryString
                });
        }

        try
        {
            // Retrieve application details
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId ?? string.Empty);
            if (application == null)
            {
                ErrorMessage = "The application could not be found.";
                return Page();
            }

            ApplicationName = await _applicationManager.GetDisplayNameAsync(application) ?? "Unknown Application";
            ClientId = request.ClientId;
            RedirectUri = request.RedirectUri;
            UserName = User.Identity?.Name;
            ReturnUrl = Request.Path + Request.QueryString;

            // Parse requested scopes
            RequestedScopes = request.GetScopes().ToList();

            // Check if user has already granted consent for this application
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var authorizationsList = new List<object>();
            await foreach (var auth in _authorizationManager.FindAsync(
                subject: userId!,
                client: await _applicationManager.GetIdAsync(application) ?? string.Empty,
                status: Statuses.Valid,
                type: AuthorizationTypes.Permanent,
                scopes: ImmutableArray.CreateRange(RequestedScopes)))
            {
                authorizationsList.Add(auth);
            }
            var authorizations = authorizationsList;

            // Check if prompt=consent is in the request
            var promptParameter = request.GetParameter("prompt")?.ToString();
            var hasConsentPrompt = promptParameter?.Contains("consent") == true;

            if (authorizations.Any() && !hasConsentPrompt)
            {
                // User already granted consent, auto-approve
                _logger.LogInformation("Auto-approving authorization for user '{UserId}' and client '{ClientId}'", userId, ClientId);
                return await GrantAuthorizationAsync(request, result.Principal);
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authorization information for client '{ClientId}'", request.ClientId);
            ErrorMessage = "An error occurred while processing the authorization request.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync(string action)
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            return BadRequest("Invalid OpenIddict request");
        }

        // Retrieve the user principal stored in the authentication cookie
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (result?.Principal == null)
        {
            return Challenge(
                authenticationSchemes: IdentityConstants.ApplicationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.Path + Request.QueryString
                });
        }

        try
        {
            if (action == "deny")
            {
                _logger.LogInformation("User denied authorization for client '{ClientId}'", request.ClientId);

                // Return an error response
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.AccessDenied,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user denied the authorization request"
                    }));
            }

            if (action == "allow")
            {
                _logger.LogInformation("User approved authorization for client '{ClientId}'", request.ClientId);
                return await GrantAuthorizationAsync(request, result.Principal);
            }

            ErrorMessage = "Invalid action specified.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing authorization action '{Action}' for client '{ClientId}'", action, request.ClientId);
            ErrorMessage = "An error occurred while processing your request.";
            return Page();
        }
    }

    private async Task<IActionResult> GrantAuthorizationAsync(OpenIddictRequest request, ClaimsPrincipal principal)
    {
        // Retrieve the application details from the database
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId ?? string.Empty);
        if (application == null)
        {
            throw new InvalidOperationException("Application not found");
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("User ID not found");
        }

        // Retrieve the permanent authorizations associated with the user and the calling client application
        var authorizationsList = new List<object>();
        await foreach (var auth in _authorizationManager.FindAsync(
            subject: userId,
            client: await _applicationManager.GetIdAsync(application) ?? string.Empty,
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: ImmutableArray.CreateRange(request.GetScopes())))
        {
            authorizationsList.Add(auth);
        }
        var authorizations = authorizationsList;

        // If no authorization exists, create one
        if (!authorizations.Any())
        {
            var authorization = await _authorizationManager.CreateAsync(
                principal: principal,
                subject: userId,
                client: await _applicationManager.GetIdAsync(application) ?? string.Empty,
                type: AuthorizationTypes.Permanent,
                scopes: request.GetScopes());

            authorizations.Add(authorization ?? throw new InvalidOperationException("Failed to create authorization"));
        }

        // Get the user to add claims
        var user = await _userManager.GetUserAsync(principal);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Create a new ClaimsPrincipal with the user's claims
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Add standard claims
        identity.AddClaim(Claims.Subject, userId);
        identity.AddClaim(Claims.Name, user.UserName ?? string.Empty);
        identity.AddClaim(Claims.Email, user.Email ?? string.Empty);

        if (user.EmailConfirmed)
        {
            identity.AddClaim(Claims.EmailVerified, "true", ClaimValueTypes.Boolean);
        }

        // Add requested claims based on scopes
        if (request.HasScope(Scopes.Profile))
        {
            identity.AddClaim(Claims.GivenName, user.FirstName);
            identity.AddClaim(Claims.FamilyName, user.LastName);
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                identity.AddClaim(Claims.Picture, user.ProfilePictureUrl);
            }
        }

        // Add roles if requested
        if (request.HasScope(Scopes.Roles))
        {
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(Claims.Role, role);
            }
        }

        // Set destinations for claims
        foreach (var claim in identity.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal));
        }

        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Set the authorization ID
        claimsPrincipal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorizations.First()) ?? string.Empty);

        // Set scopes
        claimsPrincipal.SetScopes(request.GetScopes());

        // Set resources (audiences)
        var resources = new List<string>();
        await foreach (var resource in _scopeManager.ListResourcesAsync(request.GetScopes()))
        {
            resources.Add(resource);
        }
        claimsPrincipal.SetResources(resources);

        // Sign in the user
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens
        // To allow OpenIddict to serialize them, you must attach them a destination
        switch (claim.Type)
        {
            case Claims.Name:
            case Claims.Email:
            case Claims.Subject:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                break;

            case Claims.EmailVerified:
            case Claims.GivenName:
            case Claims.FamilyName:
            case Claims.Picture:
                yield return Destinations.IdentityToken;
                break;

            case Claims.Role:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                break;

            default:
                yield return Destinations.AccessToken;
                break;
        }
    }
}
