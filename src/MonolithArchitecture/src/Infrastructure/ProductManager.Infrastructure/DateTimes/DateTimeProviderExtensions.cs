using Microsoft.Extensions.DependencyInjection;
using ProductManager.Shared.DateTimes;
namespace ProductManager.Infrastructure.DateTimes;

public static class DateTimeProviderExtensions
{
    public static IServiceCollection AddDateTimeProvider(this IServiceCollection services)
    {
        _ = services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        return services;
    }
}
