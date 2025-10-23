using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace IdentityServer.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/scopes")]
[Authorize(Roles = "Admin")]
public class ScopesAdminController : ControllerBase
{
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly ILogger<ScopesAdminController> _logger;

    public ScopesAdminController(
        IOpenIddictScopeManager scopeManager,
        ILogger<ScopesAdminController> logger)
    {
        _scopeManager = scopeManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetScopes([FromQuery] string? search = null)
    {
        try
        {
            var scopes = new List<object>();

            await foreach (var scope in _scopeManager.ListAsync())
            {
                var name = await _scopeManager.GetNameAsync(scope);

                if (!string.IsNullOrEmpty(search) &&
                    !name?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                {
                    continue;
                }

                scopes.Add(new
                {
                    Id = await _scopeManager.GetIdAsync(scope),
                    Name = name,
                    DisplayName = await _scopeManager.GetDisplayNameAsync(scope),
                    Description = await _scopeManager.GetDescriptionAsync(scope),
                    Resources = await _scopeManager.GetResourcesAsync(scope),
                    IsEnabled = true // OpenIddict doesn't have enabled/disabled flag
                });
            }

            return Ok(scopes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving scopes");
            return StatusCode(500, new { error = "Failed to retrieve scopes" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetScope(string id)
    {
        try
        {
            var scope = await _scopeManager.FindByIdAsync(id);

            if (scope == null)
            {
                return NotFound(new { error = "Scope not found" });
            }

            return Ok(new
            {
                Id = await _scopeManager.GetIdAsync(scope),
                Name = await _scopeManager.GetNameAsync(scope),
                DisplayName = await _scopeManager.GetDisplayNameAsync(scope),
                Description = await _scopeManager.GetDescriptionAsync(scope),
                Resources = await _scopeManager.GetResourcesAsync(scope)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving scope {ScopeId}", id);
            return StatusCode(500, new { error = "Failed to retrieve scope" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateScope([FromBody] CreateScopeRequest request)
    {
        try
        {
            if (await _scopeManager.FindByNameAsync(request.Name) != null)
            {
                return BadRequest(new { error = "Scope with this name already exists" });
            }

            var descriptor = new OpenIddictScopeDescriptor
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Description = request.Description
            };

            foreach (var resource in request.Resources ?? new List<string>())
            {
                descriptor.Resources.Add(resource);
            }

            await _scopeManager.CreateAsync(descriptor);
            var scope = await _scopeManager.FindByNameAsync(request.Name);
            var scopeId = await _scopeManager.GetIdAsync(scope!);

            _logger.LogInformation("Scope {ScopeName} created by admin {AdminName}", request.Name, User.Identity?.Name);

            return CreatedAtAction(nameof(GetScope), new { id = scopeId }, new { id = scopeId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating scope");
            return StatusCode(500, new { error = "Failed to create scope" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateScope(string id, [FromBody] UpdateScopeRequest request)
    {
        try
        {
            var scope = await _scopeManager.FindByIdAsync(id);

            if (scope == null)
            {
                return NotFound(new { error = "Scope not found" });
            }

            var descriptor = new OpenIddictScopeDescriptor
            {
                DisplayName = request.DisplayName,
                Description = request.Description,
                Name = await _scopeManager.GetNameAsync(scope) // Name cannot be changed
            };

            foreach (var resource in request.Resources ?? new List<string>())
            {
                descriptor.Resources.Add(resource);
            }

            await _scopeManager.UpdateAsync(scope, descriptor);

            _logger.LogInformation("Scope {ScopeId} updated by admin {AdminName}", id, User.Identity?.Name);

            return Ok(new { message = "Scope updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating scope {ScopeId}", id);
            return StatusCode(500, new { error = "Failed to update scope" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScope(string id)
    {
        try
        {
            var scope = await _scopeManager.FindByIdAsync(id);

            if (scope == null)
            {
                return NotFound(new { error = "Scope not found" });
            }

            var scopeName = await _scopeManager.GetNameAsync(scope);

            // Prevent deletion of standard OIDC scopes
            var standardScopes = new[] { "openid", "profile", "email", "address", "phone", "offline_access" };
            if (standardScopes.Contains(scopeName, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new { error = "Cannot delete standard OpenID Connect scopes" });
            }

            await _scopeManager.DeleteAsync(scope);

            _logger.LogInformation("Scope {ScopeName} deleted by admin {AdminName}", scopeName, User.Identity?.Name);

            return Ok(new { message = "Scope deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting scope {ScopeId}", id);
            return StatusCode(500, new { error = "Failed to delete scope" });
        }
    }
}

public class CreateScopeRequest
{
    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public List<string>? Resources { get; set; }
}

public class UpdateScopeRequest
{
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public List<string>? Resources { get; set; }
}
