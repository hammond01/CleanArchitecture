namespace ProductManager.Domain.Events;

public class EntityDeletedEvent<T> : IDomainEvent
    where T : Entity<string>
{
    public EntityDeletedEvent(T entity, DateTime eventDateTime)
    {
        Entity = entity;
        EventDateTime = eventDateTime;
    }

    public T Entity { get; }

    public DateTime EventDateTime { get; }
}
