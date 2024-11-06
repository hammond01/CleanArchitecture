namespace ProductManager.Domain.Events;

public class EntityCreatedEvent<T> : IDomainEvent
    where T : Entity<string>
{
    public EntityCreatedEvent(T entity, DateTime eventDateTime)
    {
        Entity = entity;
        EventDateTime = eventDateTime;
    }

    public T Entity { get; }

    public DateTime EventDateTime { get; }
}
