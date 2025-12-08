using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Infrastructure.Authorization;

/// <summary>
/// Authorization handler for permission-based access control
/// Checks if user has required permission through their roles
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionAuthorizationHandler(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Get user ID from claims
        var userId = context.User.FindFirst(c =>
            c.Type == "sub" ||
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        // Get user
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return;
        }

        // Get user's roles
        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Any())
        {
            return;
        }

        // Check if any of user's roles have the required permission
        var hasPermission = await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .AnyAsync(rp =>
                userRoles.Contains(rp.Role.Name!) &&
                rp.Permission.Name == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
