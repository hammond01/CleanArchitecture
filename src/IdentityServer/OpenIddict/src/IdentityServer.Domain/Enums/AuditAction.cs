namespace IdentityServer.Domain.Enums;

public enum AuditAction
{
    Create,
    Read,
    Update,
    Delete,
    Login,
    Logout,
    LoginFailed,
    PasswordChanged,
    PasswordReset,
    EmailConfirmed,
    TwoFactorEnabled,
    TwoFactorDisabled,
    RoleAssigned,
    RoleRemoved,
    PermissionGranted,
    PermissionRevoked,
    SessionRevoked,
    ExternalLoginAdded,
    ExternalLoginRemoved
}
