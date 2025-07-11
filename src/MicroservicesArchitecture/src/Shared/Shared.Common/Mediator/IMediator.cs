namespace Shared.Common.Mediator;

/// <summary>
/// Mediator interface for sending requests and notifications
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Send a request without response
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <param name="request">Request instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;

    /// <summary>
    /// Send a request with response
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish a notification to all handlers
    /// </summary>
    /// <param name="notification">Notification instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task Publish(object notification, CancellationToken cancellationToken = default);
}
