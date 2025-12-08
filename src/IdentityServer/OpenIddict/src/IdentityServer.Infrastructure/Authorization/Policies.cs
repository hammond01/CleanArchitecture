namespace IdentityServer.Infrastructure.Authorization;

/// <summary>
/// Contains all permission policy names used in the application
/// Use these constants instead of hardcoded strings in [Authorize(Policy = ...)]
/// </summary>
public static class Policies
{
    // User Management
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";

    // Role Management
    public const string RolesView = "roles.view";
    public const string RolesCreate = "roles.create";
    public const string RolesUpdate = "roles.update";
    public const string RolesDelete = "roles.delete";

    // Permission Management
    public const string PermissionsView = "permissions.view";
    public const string PermissionsAssign = "permissions.assign";

    // OAuth Client Management
    public const string ClientsView = "clients.view";
    public const string ClientsCreate = "clients.create";
    public const string ClientsUpdate = "clients.update";
    public const string ClientsDelete = "clients.delete";

    // Scope Management
    public const string ScopesView = "scopes.view";
    public const string ScopesCreate = "scopes.create";
    public const string ScopesUpdate = "scopes.update";
    public const string ScopesDelete = "scopes.delete";

    // Audit & Monitoring
    public const string AuditView = "audit.view";
    public const string DashboardView = "dashboard.view";

    // Session Management
    public const string SessionsView = "sessions.view";
    public const string SessionsRevoke = "sessions.revoke";
}
