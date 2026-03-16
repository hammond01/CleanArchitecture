using Auditing.Domain.Entities;

namespace Auditing.Domain.Repositories;

/// <summary>
/// Repository interface for audit log operations
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Get audit entries with pagination
    /// </summary>
    Task<List<AuditLogEntry>> GetEntriesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get audit entries by action
    /// </summary>
    Task<List<AuditLogEntry>> GetEntriesByActionAsync(string action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get audit entries by user
    /// </summary>
    Task<List<AuditLogEntry>> GetEntriesByUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create new audit log entry
    /// </summary>
    Task CreateEntryAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
}
