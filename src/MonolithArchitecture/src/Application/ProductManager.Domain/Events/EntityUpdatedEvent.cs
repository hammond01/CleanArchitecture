using ProductManager.Domain.Entities;
namespace ProductManager.Domain.Events;

public class EntityUpdatedEvent<T> : IDomainEvent
    where T : Entity<string>
{
    public EntityUpdatedEvent(T entity, DateTime eventDateTime)
    {
        Entity = entity;
        EventDateTime = eventDateTime;
    }

    public T Entity { get; }

    public DateTime EventDateTime { get; }
}
