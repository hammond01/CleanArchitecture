using Auditing.Domain.Entities;
using Auditing.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Auditing.Infrastructure.Persistence;

/// <summary>
/// Repository implementation for audit log entries
/// </summary>
public class AuditLogRepository : IAuditLogRepository
{
    private readonly AuditingDbContext _context;

    public AuditLogRepository(AuditingDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get paginated audit log entries
    /// </summary>
    public async Task<List<AuditLogEntry>> GetEntriesAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var skip = (pageNumber - 1) * pageSize;

        return await _context.AuditLogEntries
            .OrderByDescending(x => x.CreatedDateTime)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get audit log entries by action
    /// </summary>
    public async Task<List<AuditLogEntry>> GetEntriesByActionAsync(
        string action,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogEntries
            .Where(x => x.Action == action)
            .OrderByDescending(x => x.CreatedDateTime)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get audit log entries by user
    /// </summary>
    public async Task<List<AuditLogEntry>> GetEntriesByUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogEntries
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedDateTime)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Create new audit log entry
    /// </summary>
    public async Task CreateEntryAsync(
        AuditLogEntry entry,
        CancellationToken cancellationToken = default)
    {
        await _context.AuditLogEntries.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
