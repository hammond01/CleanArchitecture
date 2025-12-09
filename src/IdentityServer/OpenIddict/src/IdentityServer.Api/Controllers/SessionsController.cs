using IdentityServer.Domain.Contracts;
using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(
        ISessionService sessionService,
        UserManager<ApplicationUser> userManager,
        ILogger<SessionsController> logger)
    {
        _sessionService = sessionService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Get all active sessions for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySessions()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        try
        {
            var sessions = await _sessionService.GetUserSessionsAsync(userId);

            // Mark current session (if we can identify it by IP or other means)
            var currentIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(currentIp))
            {
                var currentSession = sessions.FirstOrDefault(s =>
                    s.IpAddress == currentIp &&
                    s.LastActivityAt > DateTime.UtcNow.AddMinutes(-5));

                if (currentSession != null)
                {
                    currentSession.IsCurrent = true;
                }
            }

            return Ok(new
            {
                total = sessions.Count,
                sessions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sessions for user {UserId}", userId);
            return StatusCode(500, new { error = "Failed to retrieve sessions" });
        }
    }

    /// <summary>
    /// Revoke a specific session
    /// </summary>
    [HttpDelete("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeSession(Guid sessionId)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        try
        {
            // Verify the session belongs to the current user
            var sessions = await _sessionService.GetUserSessionsAsync(userId);
            if (!sessions.Any(s => s.Id == sessionId))
            {
                return Forbid();
            }

            var revoked = await _sessionService.RevokeSessionAsync(sessionId, "User requested logout");

            if (!revoked)
            {
                return NotFound(new { error = "Session not found or already revoked" });
            }

            _logger.LogInformation("User {UserId} revoked session {SessionId}", userId, sessionId);

            return Ok(new { message = "Session revoked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking session {SessionId} for user {UserId}", sessionId, userId);
            return StatusCode(500, new { error = "Failed to revoke session" });
        }
    }

    /// <summary>
    /// Revoke all sessions except the current one (logout from all other devices)
    /// </summary>
    [HttpPost("revoke-others")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeOtherSessions()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        try
        {
            // Get all sessions first
            var sessions = await _sessionService.GetUserSessionsAsync(userId);

            // Identify current session by IP
            var currentIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var currentSessionId = sessions
                .Where(s => s.IpAddress == currentIp && s.LastActivityAt > DateTime.UtcNow.AddMinutes(-5))
                .OrderByDescending(s => s.LastActivityAt)
                .FirstOrDefault()?.Id;

            // Revoke all except current
            int revokedCount = 0;
            foreach (var session in sessions.Where(s => s.Id != currentSessionId))
            {
                var revoked = await _sessionService.RevokeSessionAsync(session.Id, "User logged out from other devices");
                if (revoked) revokedCount++;
            }

            _logger.LogInformation("User {UserId} revoked {Count} other sessions", userId, revokedCount);

            return Ok(new
            {
                message = $"Revoked {revokedCount} session(s)",
                revokedCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking other sessions for user {UserId}", userId);
            return StatusCode(500, new { error = "Failed to revoke sessions" });
        }
    }

    /// <summary>
    /// Revoke ALL sessions including current (full logout)
    /// </summary>
    [HttpPost("revoke-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeAllSessions()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        try
        {
            var revokedCount = await _sessionService.RevokeAllUserSessionsAsync(userId, "User logged out from all devices");

            _logger.LogInformation("User {UserId} revoked all {Count} sessions", userId, revokedCount);

            return Ok(new
            {
                message = $"Revoked all {revokedCount} session(s). Please log in again.",
                revokedCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all sessions for user {UserId}", userId);
            return StatusCode(500, new { error = "Failed to revoke sessions" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return Guid.Empty;
    }
}
