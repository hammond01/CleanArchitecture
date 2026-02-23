namespace Auditing.Domain.Entities;

/// <summary>
/// Audit log entry for tracking entity changes
/// </summary>
public class AuditLogEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = default!;

    public string Action { get; set; } = default!;

    public string ObjectId { get; set; } = default!;

    public string Log { get; set; } = default!;

    public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedDateTime { get; set; }
}
