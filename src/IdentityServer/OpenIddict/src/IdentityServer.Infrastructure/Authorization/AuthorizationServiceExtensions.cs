using IdentityServer.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring permission-based authorization
/// </summary>
public static class AuthorizationServiceExtensions
{
    /// <summary>
    /// Adds permission-based authorization to the service collection
    /// </summary>
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        // Register the permission authorization handler
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // Configure authorization with permission policies
        services.AddAuthorization(options =>
        {
            // User Management Permissions
            options.AddPolicy(Policies.UsersView, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.UsersView)));
            options.AddPolicy(Policies.UsersCreate, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.UsersCreate)));
            options.AddPolicy(Policies.UsersUpdate, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.UsersUpdate)));
            options.AddPolicy(Policies.UsersDelete, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.UsersDelete)));

            // Role Management Permissions
            options.AddPolicy(Policies.RolesView, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.RolesView)));
            options.AddPolicy(Policies.RolesCreate, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.RolesCreate)));
            options.AddPolicy(Policies.RolesUpdate, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.RolesUpdate)));
            options.AddPolicy(Policies.RolesDelete, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.RolesDelete)));

            // Permission Management
            options.AddPolicy(Policies.PermissionsView, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.PermissionsView)));
            options.AddPolicy(Policies.PermissionsAssign, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.PermissionsAssign)));

            // OAuth Client Management
            options.AddPolicy(Policies.ClientsView, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.ClientsView)));
            options.AddPolicy(Policies.ClientsCreate, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.ClientsCreate)));
            options.AddPolicy(Policies.ClientsUpdate, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.ClientsUpdate)));
            options.AddPolicy(Policies.ClientsDelete, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.ClientsDelete)));

            // Scope Management
            options.AddPolicy(Policies.ScopesView, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.ScopesView)));
            options.AddPolicy(Policies.ScopesCreate, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.ScopesCreate)));
            options.AddPolicy(Policies.ScopesUpdate, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.ScopesUpdate)));
            options.AddPolicy(Policies.ScopesDelete, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.ScopesDelete)));

            // Audit & Monitoring
            options.AddPolicy(Policies.AuditView, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.AuditView)));
            options.AddPolicy(Policies.DashboardView, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.DashboardView)));

            // Session Management
            options.AddPolicy(Policies.SessionsView, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.SessionsView)));
            options.AddPolicy(Policies.SessionsRevoke, policy =>
                policy.Requirements.Add(new PermissionRequirement(Policies.SessionsRevoke)));
        });

        return services;
    }

    /// <summary>
    /// Helper method to add a permission policy dynamically
    /// </summary>
    public static AuthorizationOptions AddPermissionPolicy(
        this AuthorizationOptions options,
        string permission)
    {
        options.AddPolicy(permission, policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));

        return options;
    }
}
