using Microsoft.Extensions.DependencyInjection;

namespace ProductManager.Shared.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileLogger(this IServiceCollection services, string logFilePath, LogLevel minimumLevel = LogLevel.Information, LogRotationOptions? rotationOptions = null)
    {
        services.AddSingleton<ILogger>(sp => new FileLogger(logFilePath, minimumLevel, rotationOptions));
        return services;
    }
}
