using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Application.Common;
namespace ProductManager.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection ApplicationConfigureServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        return services;
    }

    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
        services.AddScoped<Dispatcher>();
        var assembly = Assembly.GetExecutingAssembly();

        var assemblyTypes = assembly.GetTypes();

        foreach (var type in assemblyTypes)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(Utils.IsHandlerInterface)
                .ToList();

            if (handlerInterfaces.Count == 0)
            {
                continue;
            }

            var handlerFactory = new HandlerFactory(type);
            foreach (var interfaceType in handlerInterfaces)
            {
                services.AddTransient(interfaceType,
                implementationFactory: provider => handlerFactory.Create(provider, interfaceType)!);
            }
        }

        return services;
    }
}
