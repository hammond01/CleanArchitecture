using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Application.Dispatcher;

/// <summary>
/// Central dispatcher for commands, queries, and domain events
/// Trái tim của CQRS + Event-Driven Architecture
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Dispatch a query and return result
    /// </summary>
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatch a command and return result
    /// </summary>
    Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatch a domain event to all registered handlers
    /// </summary>
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
