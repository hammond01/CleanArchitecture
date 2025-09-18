using System.Reflection;
using CustomerManagement.Application.Common.Commands;
using CustomerManagement.Application.Common.Queries;

namespace CustomerManagement.Application.Common;

internal class HandlerFactory
{
    private readonly List<Func<object, Type, IServiceProvider, object>> _handlerFactoriesPipeline = [];

    public HandlerFactory(Type type)
    {
        AddHandlerFactory(type);
    }

    public object? Create(IServiceProvider provider, Type handlerInterfaceType)
        => _handlerFactoriesPipeline.Aggregate<Func<object, Type, IServiceProvider, object>?, object?>(null,
        func: (current, handlerFactory) => handlerFactory!(current!, handlerInterfaceType, provider));

    private void AddHandlerFactory(Type type)
    {
        _handlerFactoriesPipeline.Add((_, _, provider) => provider.GetService(type)!);
    }
}
