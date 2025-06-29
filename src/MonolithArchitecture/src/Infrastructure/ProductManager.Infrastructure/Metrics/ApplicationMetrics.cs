using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.Metrics;

/// <summary>
/// Application metrics and telemetry
/// </summary>
public class ApplicationMetrics
{
    private readonly Meter _meter;
    private readonly Counter<int> _requestCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly Counter<int> _errorCounter;
    private readonly UpDownCounter<int> _activeConnections;

    public ApplicationMetrics()
    {
        _meter = new Meter("ProductManager.Api", "1.0.0");

        _requestCounter = _meter.CreateCounter<int>(
            "api_requests_total",
            description: "Total number of API requests");

        _requestDuration = _meter.CreateHistogram<double>(
            "api_request_duration_ms",
            description: "Duration of API requests in milliseconds");

        _errorCounter = _meter.CreateCounter<int>(
            "api_errors_total",
            description: "Total number of API errors");

        _activeConnections = _meter.CreateUpDownCounter<int>(
            "api_active_connections",
            description: "Number of active connections");
    }

    public void RecordRequest(string method, string endpoint, int statusCode, double durationMs)
    {
        var tags = new TagList
        {
            { "method", method },
            { "endpoint", endpoint },
            { "status_code", statusCode.ToString() }
        };

        _requestCounter.Add(1, tags);
        _requestDuration.Record(durationMs, tags);

        if (statusCode >= 400)
        {
            _errorCounter.Add(1, tags);
        }
    }

    public void IncrementActiveConnections() => _activeConnections.Add(1);
    public void DecrementActiveConnections() => _activeConnections.Add(-1);

    public void Dispose() => _meter.Dispose();
}

/// <summary>
/// Performance monitoring middleware
/// </summary>
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApplicationMetrics _metrics;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(
        RequestDelegate next,
        ApplicationMetrics metrics,
        ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        _metrics.IncrementActiveConnections();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _metrics.DecrementActiveConnections();

            var duration = stopwatch.Elapsed.TotalMilliseconds;
            var method = context.Request.Method;
            var endpoint = context.Request.Path.Value ?? "unknown";
            var statusCode = context.Response.StatusCode;

            _metrics.RecordRequest(method, endpoint, statusCode, duration);

            // Log slow requests
            if (duration > 1000) // > 1 second
            {
                _logger.LogWarning("🐌 Slow request detected: {Method} {Endpoint} took {Duration}ms",
                    method, endpoint, duration);
            }
        }
    }
}

/// <summary>
/// Extension methods for metrics registration
/// </summary>
public static class MetricsExtensions
{
    public static IServiceCollection AddApplicationMetrics(this IServiceCollection services)
    {
        services.AddSingleton<ApplicationMetrics>();
        return services;
    }

    public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder app)
    {
        app.UseMiddleware<PerformanceMiddleware>();
        return app;
    }
}
