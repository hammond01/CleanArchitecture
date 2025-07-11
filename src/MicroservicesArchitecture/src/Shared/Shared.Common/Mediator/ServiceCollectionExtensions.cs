using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Shared.Common.Mediator;

/// <summary>
/// Extension methods for registering mediator services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add custom mediator services to the container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assemblies">Assemblies to scan for handlers</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddCustomMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        // Register the mediator
        services.AddScoped<IMediator, Mediator>();

        // Register handlers from assemblies
        foreach (var assembly in assemblies)
        {
            RegisterHandlers(services, assembly);
        }

        return services;
    }

    /// <summary>
    /// Add custom mediator services to the container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assemblyMarkerTypes">Types from assemblies to scan for handlers</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddCustomMediator(this IServiceCollection services, params Type[] assemblyMarkerTypes)
    {
        var assemblies = assemblyMarkerTypes.Select(t => t.Assembly).Distinct().ToArray();
        return services.AddCustomMediator(assemblies);
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();

            // Register request handlers without response
            var requestHandlerInterfaces = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
                .ToList();

            foreach (var handlerInterface in requestHandlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }

            // Register request handlers with response
            var requestHandlerWithResponseInterfaces = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .ToList();

            foreach (var handlerInterface in requestHandlerWithResponseInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }

            // Register notification handlers
            var notificationHandlerInterfaces = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                .ToList();

            foreach (var handlerInterface in notificationHandlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }
        }
    }
}
