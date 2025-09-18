namespace CustomerManagement.Domain.Events;

public class EntityUpdatedEvent<T> : IDomainEvent
{
  public EntityUpdatedEvent(T entity, DateTimeOffset occurredOn)
  {
    Entity = entity;
    OccurredOn = occurredOn;
  }

  public T Entity { get; }
  public DateTimeOffset OccurredOn { get; }
}
