﻿using ProductManager.Application.Common.Services;
namespace ProductManager.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection ApplicationConfigureServices(this IServiceCollection services,
        Action<Type, Type, ServiceLifetime>? configureInterceptor = null)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddMessageHandlers();
        services.RegisterService();
        services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
        return services;
    }

    private static IServiceCollection AddMessageHandlers(this IServiceCollection services)
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

    private static IServiceCollection RegisterService(this IServiceCollection service)
    {
        service.AddScoped<UserService>();
        return service;
    }
}
