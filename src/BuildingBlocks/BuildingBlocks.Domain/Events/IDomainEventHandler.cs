namespace BuildingBlocks.Domain.Events;

/// <summary>
/// Handler for domain events
/// </summary>
/// <typeparam name="TDomainEvent">Type of domain event to handle</typeparam>
public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Handle the domain event
    /// </summary>
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
