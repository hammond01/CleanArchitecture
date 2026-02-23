using System.Reflection;
using BuildingBlocks.Application.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application;

/// <summary>
/// Application layer configuration for BuildingBlocks
/// </summary>
public static class ApplicationConfiguration
{
    /// <summary>
    /// Register application services including custom Dispatcher
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register custom Dispatcher
        services.AddScoped<IDispatcher, Dispatcher.Dispatcher>();

        return services;
    }

    /// <summary>
    /// Register all handlers (Commands, Queries, Domain Events) from an assembly
    /// </summary>
    public static IServiceCollection AddHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
        // Register all command handlers
        var commandHandlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                         (i.GetGenericTypeDefinition() == typeof(CQRS.ICommandHandler<,>))))
            .Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var handlerType in commandHandlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType &&
                           i.GetGenericTypeDefinition() == typeof(CQRS.ICommandHandler<,>));

            services.AddScoped(interfaceType, handlerType);
        }

        // Register all query handlers
        var queryHandlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                         (i.GetGenericTypeDefinition() == typeof(CQRS.IQueryHandler<,>))))
            .Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var handlerType in queryHandlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType &&
                           i.GetGenericTypeDefinition() == typeof(CQRS.IQueryHandler<,>));

            services.AddScoped(interfaceType, handlerType);
        }

        // Register all domain event handlers
        Dispatcher.Dispatcher.RegisterEventHandlers(assembly, services);

        return services;
    }
}
