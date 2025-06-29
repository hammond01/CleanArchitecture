using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
namespace ProductManager.API.Logging;

public static class LoggingConfiguration
{
    public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.File(
            "logs/application-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            fileSizeLimitBytes: 5 * 1024 * 1024,// 5MB
            retainedFileCountLimit: 3,
            rollOnFileSizeLimit: true,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(1)
            )
            .CreateLogger();

        services.AddSingleton<Serilog.ILogger>(logger);

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(logger, true);
        });

        services.AddSingleton(_ => new DiagnosticContext(logger));

        return services;
    }
}
