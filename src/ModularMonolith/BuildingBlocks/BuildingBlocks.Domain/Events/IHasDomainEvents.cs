namespace BuildingBlocks.Domain.Events;

/// <summary>
/// Exposes domain events collected by an aggregate/entity.
/// </summary>
public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void AddDomainEvent(IDomainEvent domainEvent);

    void ClearDomainEvents();
}
