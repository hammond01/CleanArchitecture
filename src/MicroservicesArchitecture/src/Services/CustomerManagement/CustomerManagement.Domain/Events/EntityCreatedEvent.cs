namespace CustomerManagement.Domain.Events;

public class EntityCreatedEvent<T> : IDomainEvent
{
  public EntityCreatedEvent(T entity, DateTimeOffset occurredOn)
  {
    Entity = entity;
    OccurredOn = occurredOn;
  }

  public T Entity { get; }
  public DateTimeOffset OccurredOn { get; }
}
