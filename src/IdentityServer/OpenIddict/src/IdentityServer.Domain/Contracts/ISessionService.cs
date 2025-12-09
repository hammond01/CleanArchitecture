namespace IdentityServer.Domain.Contracts;

/// <summary>
/// Service for managing user sessions
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Create a new session when user logs in
    /// </summary>
    Task<Guid> CreateSessionAsync(Guid userId, string? refreshToken, string? ipAddress, string? userAgent);

    /// <summary>
    /// Update session activity timestamp
    /// </summary>
    Task UpdateSessionActivityAsync(Guid sessionId);

    /// <summary>
    /// Get all active sessions for a user
    /// </summary>
    Task<List<UserSessionDto>> GetUserSessionsAsync(Guid userId);

    /// <summary>
    /// Revoke a specific session
    /// </summary>
    Task<bool> RevokeSessionAsync(Guid sessionId, string reason);

    /// <summary>
    /// Revoke all sessions for a user (logout everywhere)
    /// </summary>
    Task<int> RevokeAllUserSessionsAsync(Guid userId, string reason);

    /// <summary>
    /// Revoke session by refresh token
    /// </summary>
    Task<bool> RevokeSessionByRefreshTokenAsync(string refreshToken, string reason);

    /// <summary>
    /// Clean up expired sessions
    /// </summary>
    Task<int> CleanupExpiredSessionsAsync();
}

/// <summary>
/// DTO for user session information
/// </summary>
public class UserSessionDto
{
    public Guid Id { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceInfo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrent { get; set; }
}
