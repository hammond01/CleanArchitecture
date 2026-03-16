namespace Auditing.Application.DTOs;

/// <summary>
/// DTO for audit log entry
/// </summary>
public record AuditLogEntryDto
{
    public string Id { get; init; } = null!;
    public string UserId { get; init; } = null!;
    public string Action { get; init; } = null!;
    public string ObjectId { get; init; } = null!;
    public string Log { get; init; } = null!;
    public DateTimeOffset CreatedDateTime { get; init; }
}
