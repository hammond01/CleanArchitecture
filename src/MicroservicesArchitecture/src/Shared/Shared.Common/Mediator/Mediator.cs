using Microsoft.Extensions.DependencyInjection;

namespace Shared.Common.Mediator;

/// <summary>
/// Simple Mediator implementation
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Send a request without response
    /// </summary>
    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var handler = _serviceProvider.GetService<IRequestHandler<TRequest>>();
        if (handler == null)
            throw new InvalidOperationException($"No handler registered for request type {typeof(TRequest).Name}");

        await handler.Handle(request, cancellationToken);
    }

    /// <summary>
    /// Send a request with response
    /// </summary>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        
        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");

        var method = handlerType.GetMethod("Handle");
        if (method == null)
            throw new InvalidOperationException($"Handle method not found for handler {handlerType.Name}");

        var result = method.Invoke(handler, new object[] { request, cancellationToken });
        
        if (result is Task<TResponse> taskResult)
            return await taskResult;
        
        if (result is Task task)
        {
            await task;
            var property = task.GetType().GetProperty("Result");
            return (TResponse)property?.GetValue(task)!;
        }

        return (TResponse)result!;
    }

    /// <summary>
    /// Publish a notification to all handlers
    /// </summary>
    public async Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));

        var notificationType = notification.GetType();
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
        
        var handlers = _serviceProvider.GetServices(handlerType);
        
        var tasks = new List<Task>();
        
        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("Handle");
            if (method != null)
            {
                var result = method.Invoke(handler, new object[] { notification, cancellationToken });
                if (result is Task task)
                {
                    tasks.Add(task);
                }
            }
        }

        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
        }
    }
}
