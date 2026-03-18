using System.Reflection;
using BuildingBlocks.Application.Dispatcher;
using BuildingBlocks.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            var interfaceTypes = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(CQRS.ICommandHandler<,>));

            foreach (var interfaceType in interfaceTypes)
            {
                services.TryAddScoped(interfaceType, handlerType);
            }
        }

        // Register all query handlers
        var queryHandlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                         (i.GetGenericTypeDefinition() == typeof(CQRS.IQueryHandler<,>))))
            .Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var handlerType in queryHandlerTypes)
        {
            var interfaceTypes = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(CQRS.IQueryHandler<,>));

            foreach (var interfaceType in interfaceTypes)
            {
                services.TryAddScoped(interfaceType, handlerType);
            }
        }

        // Register all domain event handlers for DI enumeration
        var domainEventHandlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                .Select(i => new { ServiceType = i, ImplementationType = t }));

        foreach (var registration in domainEventHandlerTypes)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(
                registration.ServiceType,
                registration.ImplementationType));
        }

        return services;
    }
}
