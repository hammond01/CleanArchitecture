namespace Shared.Events.Common;

/// <summary>
/// Base interface for all domain events
/// </summary>
public interface IDomainEvent
{
  Guid EventId { get; }
  DateTime OccurredOn { get; }
  string EventType { get; }
}

/// <summary>
/// Base class for all domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
  public Guid EventId { get; } = Guid.NewGuid();
  public DateTime OccurredOn { get; } = DateTime.UtcNow;
  public abstract string EventType { get; }
}

/// <summary>
/// Base interface for integration events
/// </summary>
public interface IIntegrationEvent
{
  Guid EventId { get; }
  DateTime OccurredOn { get; }
  string EventType { get; }
  string Source { get; }
}

/// <summary>
/// Base class for integration events
/// </summary>
public abstract class IntegrationEvent : IIntegrationEvent
{
  public Guid EventId { get; } = Guid.NewGuid();
  public DateTime OccurredOn { get; } = DateTime.UtcNow;
  public abstract string EventType { get; }
  public abstract string Source { get; }
}
