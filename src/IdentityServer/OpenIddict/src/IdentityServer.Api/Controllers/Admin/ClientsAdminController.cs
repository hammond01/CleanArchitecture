using IdentityServer.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace IdentityServer.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/clients")]
[Authorize]
public class ClientsAdminController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ILogger<ClientsAdminController> _logger;

    public ClientsAdminController(
        IOpenIddictApplicationManager applicationManager,
        ILogger<ClientsAdminController> logger)
    {
        _applicationManager = applicationManager;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = Policies.ClientsView)]
    public async Task<IActionResult> GetClients()
    {
        try
        {
            var clients = new List<object>();

            await foreach (var application in _applicationManager.ListAsync())
            {
                var clientId = await _applicationManager.GetClientIdAsync(application);
                var displayName = await _applicationManager.GetDisplayNameAsync(application);

                clients.Add(new
                {
                    Id = await _applicationManager.GetIdAsync(application),
                    ClientId = clientId,
                    DisplayName = displayName,
                    ClientType = await _applicationManager.GetClientTypeAsync(application),
                    RedirectUris = await _applicationManager.GetRedirectUrisAsync(application),
                    Permissions = await _applicationManager.GetPermissionsAsync(application)
                });
            }

            return Ok(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving clients");
            return StatusCode(500, new { error = "Failed to retrieve clients" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(string id)
    {
        try
        {
            var application = await _applicationManager.FindByIdAsync(id);
            if (application == null)
            {
                return NotFound(new { error = "Client not found" });
            }

            return Ok(new
            {
                Id = await _applicationManager.GetIdAsync(application),
                ClientId = await _applicationManager.GetClientIdAsync(application),
                DisplayName = await _applicationManager.GetDisplayNameAsync(application),
                Type = await _applicationManager.GetClientTypeAsync(application),
                RedirectUris = await _applicationManager.GetRedirectUrisAsync(application),
                PostLogoutRedirectUris = await _applicationManager.GetPostLogoutRedirectUrisAsync(application),
                Permissions = await _applicationManager.GetPermissionsAsync(application)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving client {ClientId}", id);
            return StatusCode(500, new { error = "Failed to retrieve client" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest request)
    {
        try
        {
            // Check if client already exists
            var existing = await _applicationManager.FindByClientIdAsync(request.ClientId);
            if (existing != null)
            {
                return BadRequest(new { error = "Client already exists" });
            }

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = request.ClientId,
                DisplayName = request.DisplayName,
                ClientType = request.RequireSecret ? OpenIddictConstants.ClientTypes.Confidential : OpenIddictConstants.ClientTypes.Public
            };

            // Add redirect URIs
            if (request.RedirectUris?.Any() == true)
            {
                foreach (var uri in request.RedirectUris.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    descriptor.RedirectUris.Add(new Uri(uri));
                }
            }

            // Add permissions
            if (request.GrantTypes?.Any() == true)
            {
                foreach (var grantType in request.GrantTypes)
                {
                    descriptor.Permissions.Add($"{OpenIddictConstants.Permissions.Prefixes.GrantType}{grantType}");
                }
            }

            if (request.Scopes?.Any() == true)
            {
                foreach (var scope in request.Scopes)
                {
                    descriptor.Permissions.Add($"{OpenIddictConstants.Permissions.Prefixes.Scope}{scope}");
                }
            }

            // Add endpoint permissions
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);

            await _applicationManager.CreateAsync(descriptor, request.ClientSecret);
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId);
            var clientId = await _applicationManager.GetIdAsync(application!);

            _logger.LogInformation("OAuth client {ClientId} created by admin {AdminName}", request.ClientId, User.Identity?.Name);

            return CreatedAtAction(nameof(GetClient), new { id = clientId }, new { id = clientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client");
            return StatusCode(500, new { error = "Failed to create client" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(string id, [FromBody] UpdateClientRequest request)
    {
        try
        {
            var application = await _applicationManager.FindByIdAsync(id);
            if (application == null)
            {
                return NotFound(new { error = "Client not found" });
            }

            await _applicationManager.UpdateAsync(application, new OpenIddictApplicationDescriptor
            {
                ClientId = await _applicationManager.GetClientIdAsync(application),
                DisplayName = request.DisplayName,
                ClientType = await _applicationManager.GetClientTypeAsync(application),
                RedirectUris = { },
                Permissions = { }
            });

            _logger.LogInformation("OAuth client {ClientId} updated by admin {AdminName}", id, User.Identity?.Name);

            return Ok(new { message = "Client updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client {ClientId}", id);
            return StatusCode(500, new { error = "Failed to update client" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(string id)
    {
        try
        {
            var application = await _applicationManager.FindByIdAsync(id);
            if (application == null)
            {
                return NotFound(new { error = "Client not found" });
            }

            await _applicationManager.DeleteAsync(application);

            _logger.LogInformation("OAuth client {ClientId} deleted by admin {AdminName}", id, User.Identity?.Name);

            return Ok(new { message = "Client deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client {ClientId}", id);
            return StatusCode(500, new { error = "Failed to delete client" });
        }
    }
}

public class CreateClientRequest
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public string? ClientSecret { get; set; }
    public bool RequireSecret { get; set; } = true;
    public List<string>? RedirectUris { get; set; }
    public List<string>? GrantTypes { get; set; }
    public List<string>? Scopes { get; set; }
}

public class UpdateClientRequest
{
    public required string DisplayName { get; set; }
    public List<string>? RedirectUris { get; set; }
    public List<string>? Scopes { get; set; }
}
