using ProductManager.Domain.Entities;

namespace ProductManager.Domain.Events;

public class EntityCreatedEvent<T> : IDomainEvent
    where T : Entity<string>
{
    public EntityCreatedEvent(T entity, DateTimeOffset eventDateTime)
    {
        Entity = entity;
        EventDateTime = eventDateTime;
    }

    public T Entity { get; }

    public DateTimeOffset EventDateTime { get; }
}
