using IdentityServer.Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers;

/// <summary>
/// Two-Factor Authentication controller
/// </summary>
[ApiController]
[Route("api/2fa")]
[Authorize]
public class TwoFactorController : ControllerBase
{
    private readonly ITwoFactorService _twoFactorService;

    public TwoFactorController(ITwoFactorService twoFactorService)
    {
        _twoFactorService = twoFactorService;
    }

    /// <summary>
    /// Get 2FA status for current user
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(TwoFactorStatus), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus()
    {
        var userId = GetCurrentUserId();
        var status = await _twoFactorService.GetStatusAsync(userId);
        return Ok(status);
    }

    /// <summary>
    /// Generate 2FA setup information (QR code, secret, backup codes)
    /// </summary>
    [HttpPost("setup")]
    [ProducesResponseType(typeof(TwoFactorSetupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Setup()
    {
        try
        {
            var userId = GetCurrentUserId();

            // Check if 2FA is already enabled
            if (await _twoFactorService.IsEnabledAsync(userId))
            {
                return BadRequest(new { error = "Two-factor authentication is already enabled" });
            }

            var setupResult = await _twoFactorService.GenerateSetupAsync(userId);
            return Ok(setupResult);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Enable 2FA with TOTP code verification
    /// </summary>
    [HttpPost("enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Enable([FromBody] EnableTwoFactorRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (await _twoFactorService.IsEnabledAsync(userId))
            {
                return BadRequest(new { error = "Two-factor authentication is already enabled" });
            }

            var success = await _twoFactorService.EnableAsync(userId, request.TotpCode);
            if (!success)
            {
                return BadRequest(new { error = "Invalid TOTP code" });
            }

            return Ok(new { message = "Two-factor authentication enabled successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Disable 2FA for current user
    /// </summary>
    [HttpPost("disable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Disable()
    {
        try
        {
            var userId = GetCurrentUserId();

            if (!await _twoFactorService.IsEnabledAsync(userId))
            {
                return BadRequest(new { error = "Two-factor authentication is not enabled" });
            }

            var success = await _twoFactorService.DisableAsync(userId);
            if (!success)
            {
                return BadRequest(new { error = "Failed to disable two-factor authentication" });
            }

            return Ok(new { message = "Two-factor authentication disabled successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Regenerate backup codes
    /// </summary>
    [HttpPost("regenerate-backup-codes")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegenerateBackupCodes()
    {
        try
        {
            var userId = GetCurrentUserId();

            if (!await _twoFactorService.IsEnabledAsync(userId))
            {
                return BadRequest(new { error = "Two-factor authentication is not enabled" });
            }

            var backupCodes = await _twoFactorService.RegenerateBackupCodesAsync(userId);
            return Ok(new { backupCodes });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Verify TOTP code (for testing during setup)
    /// </summary>
    [HttpPost("verify-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isValid = await _twoFactorService.VerifyCodeAsync(userId, request.Code);

            if (!isValid)
            {
                return BadRequest(new { error = "Invalid TOTP code" });
            }

            return Ok(new { message = "TOTP code verified successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Verify backup code (for testing)
    /// </summary>
    [HttpPost("verify-backup-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyBackupCode([FromBody] VerifyCodeRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isValid = await _twoFactorService.VerifyBackupCodeAsync(userId, request.Code);

            if (!isValid)
            {
                return BadRequest(new { error = "Invalid backup code" });
            }

            return Ok(new { message = "Backup code verified and consumed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity");
        }
        return userId;
    }
}

/// <summary>
/// Request model for enabling 2FA
/// </summary>
public class EnableTwoFactorRequest
{
    public string TotpCode { get; set; } = string.Empty;
}

/// <summary>
/// Request model for verifying codes
/// </summary>
public class VerifyCodeRequest
{
    public string Code { get; set; } = string.Empty;
}