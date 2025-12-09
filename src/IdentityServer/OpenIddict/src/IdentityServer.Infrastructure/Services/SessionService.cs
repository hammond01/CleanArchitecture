using IdentityServer.Domain.Contracts;
using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UAParser;

namespace IdentityServer.Infrastructure.Services;

/// <summary>
/// Session management service implementation
/// </summary>
public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SessionService> _logger;

    public SessionService(ApplicationDbContext context, ILogger<SessionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> CreateSessionAsync(Guid userId, string? refreshToken, string? ipAddress, string? userAgent)
    {
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RefreshToken = refreshToken,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            DeviceInfo = ParseDeviceInfo(userAgent),
            CreatedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.UserSessions.Add(session);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created session {SessionId} for user {UserId} from {IpAddress}",
            session.Id, userId, ipAddress);

        return session.Id;
    }

    public async Task UpdateSessionActivityAsync(Guid sessionId)
    {
        var session = await _context.UserSessions.FindAsync(sessionId);
        if (session != null && session.IsActive)
        {
            session.LastActivityAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<UserSessionDto>> GetUserSessionsAsync(Guid userId)
    {
        var sessions = await _context.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .OrderByDescending(s => s.LastActivityAt)
            .Select(s => new UserSessionDto
            {
                Id = s.Id,
                IpAddress = s.IpAddress,
                UserAgent = s.UserAgent,
                DeviceInfo = s.DeviceInfo,
                CreatedAt = s.CreatedAt,
                LastActivityAt = s.LastActivityAt,
                IsActive = s.IsActive,
                IsCurrent = false // Will be set by controller based on current session
            })
            .ToListAsync();

        return sessions;
    }

    public async Task<bool> RevokeSessionAsync(Guid sessionId, string reason)
    {
        var session = await _context.UserSessions.FindAsync(sessionId);
        if (session == null)
        {
            _logger.LogWarning("Session {SessionId} not found for revocation", sessionId);
            return false;
        }

        if (!session.IsActive)
        {
            _logger.LogInformation("Session {SessionId} already revoked", sessionId);
            return false;
        }

        session.IsActive = false;
        session.RevokedAt = DateTime.UtcNow;
        session.RevokedReason = reason;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Revoked session {SessionId} for user {UserId}. Reason: {Reason}",
            sessionId, session.UserId, reason);

        return true;
    }

    public async Task<int> RevokeAllUserSessionsAsync(Guid userId, string reason)
    {
        var activeSessions = await _context.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync();

        if (!activeSessions.Any())
        {
            _logger.LogInformation("No active sessions found for user {UserId}", userId);
            return 0;
        }

        var now = DateTime.UtcNow;
        foreach (var session in activeSessions)
        {
            session.IsActive = false;
            session.RevokedAt = now;
            session.RevokedReason = reason;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Revoked {Count} sessions for user {UserId}. Reason: {Reason}",
            activeSessions.Count, userId, reason);

        return activeSessions.Count;
    }

    public async Task<bool> RevokeSessionByRefreshTokenAsync(string refreshToken, string reason)
    {
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.IsActive);

        if (session == null)
        {
            _logger.LogWarning("Session with refresh token not found or already revoked");
            return false;
        }

        session.IsActive = false;
        session.RevokedAt = DateTime.UtcNow;
        session.RevokedReason = reason;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Revoked session {SessionId} by refresh token. Reason: {Reason}",
            session.Id, reason);

        return true;
    }

    public async Task<int> CleanupExpiredSessionsAsync()
    {
        // Sessions older than 30 days or with expired refresh tokens
        var cutoffDate = DateTime.UtcNow.AddDays(-30);

        var expiredSessions = await _context.UserSessions
            .Where(s => s.IsActive &&
                       (s.CreatedAt < cutoffDate ||
                        (s.RefreshTokenExpiresAt != null && s.RefreshTokenExpiresAt < DateTime.UtcNow)))
            .ToListAsync();

        if (!expiredSessions.Any())
        {
            return 0;
        }

        var now = DateTime.UtcNow;
        foreach (var session in expiredSessions)
        {
            session.IsActive = false;
            session.RevokedAt = now;
            session.RevokedReason = "Expired or inactive for 30 days";
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Cleaned up {Count} expired sessions", expiredSessions.Count);

        return expiredSessions.Count;
    }

    private string? ParseDeviceInfo(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return "Unknown Device";

        try
        {
            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(userAgent);

            var device = clientInfo.Device.Family != "Other"
                ? clientInfo.Device.Family
                : "Desktop";

            var os = $"{clientInfo.OS.Family} {clientInfo.OS.Major}";
            var browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}";

            return $"{device} - {os} - {browser}";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse user agent: {UserAgent}", userAgent);
            return "Unknown Device";
        }
    }
}
