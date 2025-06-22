using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Queries;
using ProductManager.Domain.Events;

namespace ProductManager.Application.Common;

public class Dispatcher
{
    // ReSharper disable once CollectionNeverUpdated.Local
    private static readonly List<Type> _eventHandlers = [];
    private readonly IServiceProvider _serviceProvider;
    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public static void RegisterEventHandlers(Assembly assembly, IServiceCollection services)
    {
        var types = assembly.GetTypes()
            .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
            .ToList();

        foreach (var type in types)
        {
            services.AddTransient(type);
        }
        _eventHandlers.AddRange(types);
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
