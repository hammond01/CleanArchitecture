using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using CustomerManagement.Application.Common.Commands;
using CustomerManagement.Application.Common.Queries;

namespace CustomerManagement.Application.Common;

public class MessageDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public MessageDispatcher(IServiceProvider serviceProvider)
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
}
