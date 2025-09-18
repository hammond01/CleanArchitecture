using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using CustomerManagement.Application.Services;
using CustomerManagement.Application.Mappers;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Domain.Services;
using CustomerManagement.Application.Common;

namespace CustomerManagement.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddCustomerManagementApplication(this IServiceCollection services)
    {
        services.AddMessageHandlers();

        // Register services
        services.AddScoped<MessageDispatcher>();
        services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
        // Note: IRepository registration moved to Infrastructure layer

        // Register mappers
        services.AddScoped<ICustomerMapper, CustomerMapper>();    // Register event handlers
        Dispatcher.RegisterEventHandlers(Assembly.GetExecutingAssembly(), services);

        return services;
    }

    private static void AddMessageHandlers(this IServiceCollection services)
    {
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
    }
}
