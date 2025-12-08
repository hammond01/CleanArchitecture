using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Infrastructure.Authorization;

/// <summary>
/// Seeds permissions and assigns them to default roles
/// </summary>
public class PermissionSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PermissionSeeder>>();

        // Define all permissions
        var permissions = new List<Permission>
        {
            // User Management
            new Permission { Id = Guid.NewGuid(), Name = "users.view", DisplayName = "View Users", Description = "Can view user list and details", Category = "User Management" },
            new Permission { Id = Guid.NewGuid(), Name = "users.create", DisplayName = "Create Users", Description = "Can create new users", Category = "User Management" },
            new Permission { Id = Guid.NewGuid(), Name = "users.update", DisplayName = "Update Users", Description = "Can update user information", Category = "User Management" },
            new Permission { Id = Guid.NewGuid(), Name = "users.delete", DisplayName = "Delete Users", Description = "Can delete users", Category = "User Management" },

            // Role Management
            new Permission { Id = Guid.NewGuid(), Name = "roles.view", DisplayName = "View Roles", Description = "Can view role list and details", Category = "Role Management" },
            new Permission { Id = Guid.NewGuid(), Name = "roles.create", DisplayName = "Create Roles", Description = "Can create new roles", Category = "Role Management" },
            new Permission { Id = Guid.NewGuid(), Name = "roles.update", DisplayName = "Update Roles", Description = "Can update role information", Category = "Role Management" },
            new Permission { Id = Guid.NewGuid(), Name = "roles.delete", DisplayName = "Delete Roles", Description = "Can delete roles", Category = "Role Management" },

            // Permission Management
            new Permission { Id = Guid.NewGuid(), Name = "permissions.view", DisplayName = "View Permissions", Description = "Can view permissions", Category = "Permission Management" },
            new Permission { Id = Guid.NewGuid(), Name = "permissions.assign", DisplayName = "Assign Permissions", Description = "Can assign permissions to roles", Category = "Permission Management" },

            // OAuth Client Management
            new Permission { Id = Guid.NewGuid(), Name = "clients.view", DisplayName = "View Clients", Description = "Can view OAuth clients", Category = "OAuth Management" },
            new Permission { Id = Guid.NewGuid(), Name = "clients.create", DisplayName = "Create Clients", Description = "Can create OAuth clients", Category = "OAuth Management" },
            new Permission { Id = Guid.NewGuid(), Name = "clients.update", DisplayName = "Update Clients", Description = "Can update OAuth clients", Category = "OAuth Management" },
            new Permission { Id = Guid.NewGuid(), Name = "clients.delete", DisplayName = "Delete Clients", Description = "Can delete OAuth clients", Category = "OAuth Management" },

            // Scope Management
            new Permission { Id = Guid.NewGuid(), Name = "scopes.view", DisplayName = "View Scopes", Description = "Can view OAuth scopes", Category = "OAuth Management" },
            new Permission { Id = Guid.NewGuid(), Name = "scopes.create", DisplayName = "Create Scopes", Description = "Can create OAuth scopes", Category = "OAuth Management" },
            new Permission { Id = Guid.NewGuid(), Name = "scopes.update", DisplayName = "Update Scopes", Description = "Can update OAuth scopes", Category = "OAuth Management" },
            new Permission { Id = Guid.NewGuid(), Name = "scopes.delete", DisplayName = "Delete Scopes", Description = "Can delete OAuth scopes", Category = "OAuth Management" },

            // Audit & Monitoring
            new Permission { Id = Guid.NewGuid(), Name = "audit.view", DisplayName = "View Audit Logs", Description = "Can view audit logs", Category = "Monitoring" },
            new Permission { Id = Guid.NewGuid(), Name = "dashboard.view", DisplayName = "View Dashboard", Description = "Can view admin dashboard", Category = "Monitoring" },

            // Session Management
            new Permission { Id = Guid.NewGuid(), Name = "sessions.view", DisplayName = "View Sessions", Description = "Can view user sessions", Category = "Session Management" },
            new Permission { Id = Guid.NewGuid(), Name = "sessions.revoke", DisplayName = "Revoke Sessions", Description = "Can revoke user sessions", Category = "Session Management" },
        };

        // Add permissions if they don't exist
        foreach (var permission in permissions)
        {
            if (!await context.Permissions.AnyAsync(p => p.Name == permission.Name))
            {
                context.Permissions.Add(permission);
                logger.LogInformation("Added permission: {Permission}", permission.Name);
            }
        }

        await context.SaveChangesAsync();

        // Assign all permissions to Admin role
        var adminRole = await roleManager.FindByNameAsync("Admin");
        if (adminRole != null)
        {
            var allPermissions = await context.Permissions.ToListAsync();

            foreach (var permission in allPermissions)
            {
                if (!await context.RolePermissions.AnyAsync(rp =>
                    rp.RoleId == adminRole.Id && rp.PermissionId == permission.Id))
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = permission.Id,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            await context.SaveChangesAsync();
            logger.LogInformation("✅ Assigned all permissions to Admin role");
        }

        // Assign read-only permissions to User role
        var userRole = await roleManager.FindByNameAsync("User");
        if (userRole != null)
        {
            var readPermissions = await context.Permissions
                .Where(p => p.Name.EndsWith(".view"))
                .ToListAsync();

            foreach (var permission in readPermissions)
            {
                if (!await context.RolePermissions.AnyAsync(rp =>
                    rp.RoleId == userRole.Id && rp.PermissionId == permission.Id))
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = userRole.Id,
                        PermissionId = permission.Id,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            await context.SaveChangesAsync();
            logger.LogInformation("✅ Assigned view permissions to User role");
        }

        logger.LogInformation("✅ Permission seeding completed");
    }
}
