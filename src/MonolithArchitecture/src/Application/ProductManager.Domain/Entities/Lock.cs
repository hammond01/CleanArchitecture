namespace ProductManager.Domain.Entities;

public class Lock
{
    public string EntityId { get; set; } = default!;

    public string EntityName { get; set; } = default!;

    public string? OwnerId { get; set; }

    public DateTimeOffset? AcquiredDateTime { get; set; }

    public DateTimeOffset? ExpiredDateTime { get; set; }
}
