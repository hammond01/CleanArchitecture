namespace Shared.Common.Mediator;

/// <summary>
/// Marker interface for requests without response
/// </summary>
public interface IRequest
{
}

/// <summary>
/// Interface for requests with response
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IRequest<out TResponse>
{
}

/// <summary>
/// Interface for request handlers without response
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
    Task Handle(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for request handlers with response
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for notification
/// </summary>
public interface INotification
{
}

/// <summary>
/// Interface for notification handlers
/// </summary>
/// <typeparam name="TNotification">Notification type</typeparam>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}
