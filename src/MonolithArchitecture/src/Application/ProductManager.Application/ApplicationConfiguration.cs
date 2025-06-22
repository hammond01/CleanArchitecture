using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Application.Common;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Decorators;

namespace ProductManager.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection ApplicationConfigureServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });        // Configure Mapster
        MappingConfig.ConfigureMappings();

        services.AddScoped<UserService>();
        services.AddMessageHandlers();
        services.RegisterService();
        services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
        return services;
    }

    private static void AddMessageHandlers(this IServiceCollection services)
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

        Dispatcher.RegisterEventHandlers(assembly, services);
    }

    private static void RegisterService(this IServiceCollection service) => service.AddScoped<UserService>();
}
