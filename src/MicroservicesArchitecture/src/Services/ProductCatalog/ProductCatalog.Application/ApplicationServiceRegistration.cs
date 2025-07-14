using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using Shared.Common.Mediator;

namespace ProductCatalog.Application;

/// <summary>
/// Application layer dependency injection configuration
/// </summary>
public static class ApplicationServiceRegistration
{
    /// <summary>
    /// Register application layer services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register custom MediatR
        services.AddMediatR();

        // Register request handlers
        RegisterRequestHandlers(services);

        return services;
    }

    private static void RegisterRequestHandlers(IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register all request handlers
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => IsHandlerInterface(i)))
            .Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var handlerType in handlerTypes)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(i => IsHandlerInterface(i));

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, handlerType);
            }
        }
    }

    private static bool IsHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericType = type.GetGenericTypeDefinition();
        return genericType == typeof(IRequestHandler<>) ||
               genericType == typeof(IRequestHandler<,>) ||
               genericType == typeof(INotificationHandler<>);
    }
}
