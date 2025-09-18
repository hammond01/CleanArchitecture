namespace CustomerManagement.Domain.Events;

public class EntityDeletedEvent<T> : IDomainEvent
{
  public EntityDeletedEvent(T entity, DateTimeOffset occurredOn)
  {
    Entity = entity;
    OccurredOn = occurredOn;
  }

  public T Entity { get; }
  public DateTimeOffset OccurredOn { get; }
}
