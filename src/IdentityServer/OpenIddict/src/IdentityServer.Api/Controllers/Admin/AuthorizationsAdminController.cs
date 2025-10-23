using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace IdentityServer.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/authorizations")]
[Authorize(Roles = "Admin")]
public class AuthorizationsAdminController : ControllerBase
{
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ILogger<AuthorizationsAdminController> _logger;

    public AuthorizationsAdminController(
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictApplicationManager applicationManager,
        ILogger<AuthorizationsAdminController> logger)
    {
        _authorizationManager = authorizationManager;
        _applicationManager = applicationManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuthorizations(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? type = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var allAuthorizations = new List<object>();

            await foreach (var authorization in _authorizationManager.ListAsync())
            {
                var subject = await _authorizationManager.GetSubjectAsync(authorization);
                var applicationId = await _authorizationManager.GetApplicationIdAsync(authorization);

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    var matchesSearch = subject?.Contains(search, StringComparison.OrdinalIgnoreCase) == true;
                    if (applicationId != null)
                    {
                        var app = await _applicationManager.FindByIdAsync(applicationId.ToString()!);
                        if (app != null)
                        {
                            var clientId = await _applicationManager.GetClientIdAsync(app);
                            matchesSearch = matchesSearch || clientId?.Contains(search, StringComparison.OrdinalIgnoreCase) == true;
                        }
                    }
                    if (!matchesSearch) continue;
                }

                var authType = await _authorizationManager.GetTypeAsync(authorization);
                var authStatus = await _authorizationManager.GetStatusAsync(authorization);

                // Apply type filter
                if (!string.IsNullOrEmpty(type) && authType != type)
                {
                    continue;
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(status) && authStatus != status)
                {
                    continue;
                }

                // Get application name
                string? applicationName = null;
                if (applicationId != null)
                {
                    var app = await _applicationManager.FindByIdAsync(applicationId.ToString()!);
                    if (app != null)
                    {
                        applicationName = await _applicationManager.GetDisplayNameAsync(app);
                    }
                }

                allAuthorizations.Add(new
                {
                    Id = await _authorizationManager.GetIdAsync(authorization),
                    Subject = subject,
                    ApplicationId = applicationId?.ToString(),
                    ApplicationName = applicationName,
                    Type = authType,
                    Status = authStatus,
                    Scopes = await _authorizationManager.GetScopesAsync(authorization),
                    CreationDate = await _authorizationManager.GetCreationDateAsync(authorization)
                });
            }

            var totalCount = allAuthorizations.Count;
            var items = allAuthorizations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authorizations");
            return StatusCode(500, new { error = "Failed to retrieve authorizations" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthorization(string id)
    {
        try
        {
            var authorization = await _authorizationManager.FindByIdAsync(id);

            if (authorization == null)
            {
                return NotFound(new { error = "Authorization not found" });
            }

            var applicationId = await _authorizationManager.GetApplicationIdAsync(authorization);
            string? applicationName = null;

            if (applicationId != null)
            {
                var app = await _applicationManager.FindByIdAsync(applicationId.ToString()!);
                if (app != null)
                {
                    applicationName = await _applicationManager.GetDisplayNameAsync(app);
                }
            }

            return Ok(new
            {
                Id = await _authorizationManager.GetIdAsync(authorization),
                Subject = await _authorizationManager.GetSubjectAsync(authorization),
                ApplicationId = applicationId?.ToString(),
                ApplicationName = applicationName,
                Type = await _authorizationManager.GetTypeAsync(authorization),
                Status = await _authorizationManager.GetStatusAsync(authorization),
                Scopes = await _authorizationManager.GetScopesAsync(authorization),
                CreationDate = await _authorizationManager.GetCreationDateAsync(authorization)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authorization {AuthorizationId}", id);
            return StatusCode(500, new { error = "Failed to retrieve authorization" });
        }
    }

    [HttpPost("{id}/revoke")]
    public async Task<IActionResult> RevokeAuthorization(string id)
    {
        try
        {
            var authorization = await _authorizationManager.FindByIdAsync(id);

            if (authorization == null)
            {
                return NotFound(new { error = "Authorization not found" });
            }

            // Update authorization with revoked status
            var descriptor = new OpenIddictAuthorizationDescriptor();
            await _authorizationManager.PopulateAsync(descriptor, authorization);
            descriptor.Status = OpenIddictConstants.Statuses.Revoked;
            await _authorizationManager.UpdateAsync(authorization, descriptor);

            var subject = await _authorizationManager.GetSubjectAsync(authorization);
            _logger.LogInformation("Authorization {AuthorizationId} for subject {Subject} revoked by admin {AdminName}",
                id, subject, User.Identity?.Name);

            return Ok(new { message = "Authorization revoked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking authorization {AuthorizationId}", id);
            return StatusCode(500, new { error = "Failed to revoke authorization" });
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthorization(string id)
    {
        try
        {
            var authorization = await _authorizationManager.FindByIdAsync(id);

            if (authorization == null)
            {
                return NotFound(new { error = "Authorization not found" });
            }

            await _authorizationManager.DeleteAsync(authorization);

            _logger.LogInformation("Authorization {AuthorizationId} deleted by admin {AdminName}", id, User.Identity?.Name);

            return Ok(new { message = "Authorization deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting authorization {AuthorizationId}", id);
            return StatusCode(500, new { error = "Failed to delete authorization" });
        }
    }
}
