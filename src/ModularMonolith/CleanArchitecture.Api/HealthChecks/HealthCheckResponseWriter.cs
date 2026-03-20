using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArchitecture.Api.HealthChecks;

public static class HealthCheckResponseWriter
{
    private static readonly string ApplicationVersion = ResolveApplicationVersion();

    public static Task WriteJsonAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = report.Status.ToString(),
            version = ApplicationVersion,
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.TotalMilliseconds,
                error = entry.Value.Exception?.Message
            })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private static string ResolveApplicationVersion()
    {
        var runtimeVersion = Environment.GetEnvironmentVariable("APP_BUILD_VERSION");
        if (!string.IsNullOrWhiteSpace(runtimeVersion))
        {
            return runtimeVersion;
        }

        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
               ?? assembly.GetName().Version?.ToString()
               ?? "unknown";
    }
}
