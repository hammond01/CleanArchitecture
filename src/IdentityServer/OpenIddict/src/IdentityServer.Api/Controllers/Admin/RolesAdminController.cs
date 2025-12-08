using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/roles")]
[Authorize]
public class RolesAdminController : ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RolesAdminController> _logger;

    public RolesAdminController(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<RolesAdminController> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = Policies.RolesView)]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            var rolesList = await _roleManager.Roles.ToListAsync();

            // Load user counts separately to avoid .Result in LINQ
            var rolesWithDetails = new List<object>();
            foreach (var role in rolesList)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                rolesWithDetails.Add(new
                {
                    role.Id,
                    role.Name,
                    role.Description,
                    UserCount = usersInRole.Count,
                    Permissions = role.RolePermissions.Select(rp => rp.Permission.Name).ToList()
                });
            }

            return Ok(rolesWithDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(500, new { error = "Failed to retrieve roles" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound(new { error = "Role not found" });
            }

            return Ok(new
            {
                role.Id,
                role.Name,
                role.Description
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role {RoleId}", id);
            return StatusCode(500, new { error = "Failed to retrieve role" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        try
        {
            if (await _roleManager.RoleExistsAsync(request.Name))
            {
                return BadRequest(new { error = "Role already exists" });
            }

            var role = new ApplicationRole
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            _logger.LogInformation("Role {RoleName} created by admin {AdminName}", role.Name, User.Identity?.Name);

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, new { role.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(500, new { error = "Failed to create role" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound(new { error = "Role not found" });
            }

            role.Description = request.Description;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            _logger.LogInformation("Role {RoleId} updated by admin {AdminName}", role.Id, User.Identity?.Name);

            return Ok(new { message = "Role updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            return StatusCode(500, new { error = "Failed to update role" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound(new { error = "Role not found" });
            }

            // Prevent deletion of system roles
            if (role.Name == "Admin" || role.Name == "User")
            {
                return BadRequest(new { error = "Cannot delete system roles" });
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            _logger.LogInformation("Role {RoleId} deleted by admin {AdminName}", role.Id, User.Identity?.Name);

            return Ok(new { message = "Role deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return StatusCode(500, new { error = "Failed to delete role" });
        }
    }
}

public class CreateRoleRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateRoleRequest
{
    public string? Description { get; set; }
}
