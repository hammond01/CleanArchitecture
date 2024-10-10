using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Queries;
namespace ProductManager.Application.Common;

public class Dispatcher
{
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
        Task<T> result = handler.HandleAsync((dynamic)query, cancellationToken);

        return await result;
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
