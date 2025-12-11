using IdentityServer.Domain.Contracts;
using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using System.Security.Cryptography;

namespace IdentityServer.Infrastructure.Services;

/// <summary>
/// Two-Factor Authentication service implementation
/// </summary>
public class TwoFactorService : ITwoFactorService
{
    private readonly ApplicationDbContext _context;

    public TwoFactorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TwoFactorSetupResult> GenerateSetupAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found", nameof(userId));

        // Generate TOTP secret
        var secret = Base32Encoding.ToString(RandomNumberGenerator.GetBytes(32));

        // Generate QR code URI
        var issuer = "IdentityServer";
        var accountName = user.Email ?? user.UserName ?? "User";
        var qrCodeUri = $"otpauth://totp/{issuer}:{accountName}?secret={secret}&issuer={issuer}";

        // Generate backup codes
        var backupCodes = GenerateBackupCodes();

        return new TwoFactorSetupResult
        {
            Secret = secret,
            QrCodeUri = qrCodeUri,
            ManualEntryKey = secret,
            BackupCodes = backupCodes
        };
    }

    public async Task<bool> EnableAsync(Guid userId, string totpCode)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found", nameof(userId));

        if (string.IsNullOrEmpty(user.TwoFactorSecret))
            throw new InvalidOperationException("2FA setup not initiated");

        // Verify the TOTP code
        if (!VerifyTotpCode(user.TwoFactorSecret, totpCode))
            return false;

        // Generate backup codes
        var backupCodes = GenerateBackupCodes();

        // Enable 2FA
        user.TwoFactorEnabled = true;
        user.TwoFactorEnabledAt = DateTime.UtcNow;
        user.BackupCodes = backupCodes;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VerifyCodeAsync(Guid userId, string totpCode)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
            return false;

        return VerifyTotpCode(user.TwoFactorSecret, totpCode);
    }

    public async Task<bool> VerifyBackupCodeAsync(Guid userId, string backupCode)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.TwoFactorEnabled || user.BackupCodes.Count == 0)
            return false;

        // Find and remove the backup code
        var codeIndex = user.BackupCodes.FindIndex(code => code == backupCode);
        if (codeIndex == -1)
            return false;

        user.BackupCodes.RemoveAt(codeIndex);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DisableAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.TwoFactorEnabled = false;
        user.TwoFactorSecret = null;
        user.TwoFactorEnabledAt = null;
        user.BackupCodes.Clear();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<string>> RegenerateBackupCodesAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.TwoFactorEnabled)
            throw new InvalidOperationException("2FA not enabled for user");

        var backupCodes = GenerateBackupCodes();
        user.BackupCodes = backupCodes;

        await _context.SaveChangesAsync();
        return backupCodes;
    }

    public async Task<bool> IsEnabledAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.TwoFactorEnabled ?? false;
    }

    public async Task<TwoFactorStatus> GetStatusAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found", nameof(userId));

        return new TwoFactorStatus
        {
            IsEnabled = user.TwoFactorEnabled,
            EnabledAt = user.TwoFactorEnabledAt,
            HasBackupCodes = user.BackupCodes.Any(),
            BackupCodesCount = user.BackupCodes.Count
        };
    }

    private bool VerifyTotpCode(string secret, string code)
    {
        try
        {
            var secretBytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(secretBytes);

            // Allow for time drift (30 seconds window)
            var validCodes = new[]
            {
                totp.ComputeTotp(),
                totp.ComputeTotp(DateTime.UtcNow.AddSeconds(-30)),
                totp.ComputeTotp(DateTime.UtcNow.AddSeconds(30))
            };

            return validCodes.Contains(code);
        }
        catch
        {
            return false;
        }
    }

    private List<string> GenerateBackupCodes()
    {
        var codes = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            codes.Add(code);
        }
        return codes;
    }
}
