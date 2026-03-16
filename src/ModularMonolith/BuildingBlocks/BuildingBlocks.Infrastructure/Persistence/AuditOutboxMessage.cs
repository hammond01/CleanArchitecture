namespace BuildingBlocks.Infrastructure.Persistence;

public class AuditOutboxMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string ModuleName { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public string EntityId { get; set; } = null!;

    public string ChangeType { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string Log { get; set; } = null!;

    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ProcessedDateTime { get; set; }

    public int AttemptCount { get; set; }

    public string? LastError { get; set; }
}
