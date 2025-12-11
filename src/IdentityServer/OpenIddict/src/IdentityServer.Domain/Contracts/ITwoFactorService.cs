using IdentityServer.Domain.Entities;

namespace IdentityServer.Domain.Contracts;

/// <summary>
/// Two-Factor Authentication service interface
/// </summary>
public interface ITwoFactorService
{
    /// <summary>
    /// Generate TOTP secret and setup information for user
    /// </summary>
    Task<TwoFactorSetupResult> GenerateSetupAsync(Guid userId);

    /// <summary>
    /// Enable 2FA for user with verified TOTP code
    /// </summary>
    Task<bool> EnableAsync(Guid userId, string totpCode);

    /// <summary>
    /// Verify TOTP code
    /// </summary>
    Task<bool> VerifyCodeAsync(Guid userId, string totpCode);

    /// <summary>
    /// Verify backup code and consume it
    /// </summary>
    Task<bool> VerifyBackupCodeAsync(Guid userId, string backupCode);

    /// <summary>
    /// Disable 2FA for user
    /// </summary>
    Task<bool> DisableAsync(Guid userId);

    /// <summary>
    /// Regenerate backup codes for user
    /// </summary>
    Task<List<string>> RegenerateBackupCodesAsync(Guid userId);

    /// <summary>
    /// Check if user has 2FA enabled
    /// </summary>
    Task<bool> IsEnabledAsync(Guid userId);

    /// <summary>
    /// Get 2FA status for user
    /// </summary>
    Task<TwoFactorStatus> GetStatusAsync(Guid userId);
}

/// <summary>
/// 2FA setup result
/// </summary>
public class TwoFactorSetupResult
{
    public string Secret { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    public List<string> BackupCodes { get; set; } = new();
}

/// <summary>
/// 2FA status information
/// </summary>
public class TwoFactorStatus
{
    public bool IsEnabled { get; set; }
    public DateTime? EnabledAt { get; set; }
    public bool HasBackupCodes { get; set; }
    public int BackupCodesCount { get; set; }
}
