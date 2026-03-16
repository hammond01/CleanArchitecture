namespace Identity.Infrastructure.Services;

/// <summary>
/// Identity security settings
/// </summary>
public sealed class IdentitySecuritySettings
{
    public int MaxLoginAttempts { get; set; } = 5;
    public int LockoutMinutes { get; set; } = 15;
    public int EmailConfirmationTokenHours { get; set; } = 24;
    public int PasswordResetTokenMinutes { get; set; } = 60;
    public int ResetRequestCooldownMinutes { get; set; } = 5;
}
