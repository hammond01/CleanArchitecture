namespace ProductManager.Domain.Entities;

public class OutboxEvent : OutboxEventBase
{
}
public class ArchivedOutboxEvent : OutboxEventBase
{
}
public abstract class OutboxEventBase : Entity<string>
{
    public string EventType { get; set; } = default!;

    public string TriggeredById { get; set; } = default!;

    public string ObjectId { get; set; } = default!;

    public string Message { get; set; } = default!;

    public bool Published { get; set; }
}
