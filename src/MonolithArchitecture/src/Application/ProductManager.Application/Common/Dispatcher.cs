using ProductManager.Domain.Events;
namespace ProductManager.Application.Common;

public class Dispatcher
{
    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly List<Type> _eventHandlers = [];
    private readonly IServiceProvider _serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task<T> DispatchAsync<T>(IQuery<T> query, CancellationToken cancellationToken = default)
    {
        var type = typeof(IQueryHandler<,>);
        Type[] typeArgs = [query.GetType(), typeof(T)];
        var handlerType = type.MakeGenericType(typeArgs);

        dynamic handler = _serviceProvider.GetService(handlerType)!;
        return await handler.HandleAsync((dynamic)query, cancellationToken);
    }
    public async Task<T> DispatchAsync<T>(ICommand<T> command, CancellationToken cancellationToken = default)
    {
        var type = typeof(ICommandHandler<,>);
        Type[] typeArgs = [command.GetType(), typeof(T)];
        var handlerType = type.MakeGenericType(typeArgs);

        dynamic handler = _serviceProvider.GetService(handlerType)!;
        return await handler.HandleAsync((dynamic)command, cancellationToken);
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        foreach (var handler in from handlerType in _eventHandlers
                                let canHandleEvent = handlerType.GetInterfaces()
                                    .Any(x => x.IsGenericType
                                              && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
                                              && x.GenericTypeArguments[0] == domainEvent.GetType())
                                where canHandleEvent
                                select _serviceProvider.GetService(handlerType)!)
        {
            await ((dynamic)handler).HandleAsync((dynamic)domainEvent, cancellationToken);
        }
    }
}
