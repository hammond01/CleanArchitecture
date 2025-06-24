// Advanced Rate Limiting Middleware
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ProductManager.Infrastructure.Middleware;

/// <summary>
/// Advanced rate limiting middleware with IP-based and user-based throttling
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;

    public RateLimitingMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        ILogger<RateLimitingMiddleware> logger,
        RateLimitOptions options)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting for certain paths
        if (ShouldSkipRateLimit(context))
        {
            await _next(context);
            return;
        }

        var identifier = GetClientIdentifier(context);
        var key = $"rate_limit_{identifier}";

        var requests = _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _options.Window;
            return new List<DateTime>();
        });

        // Clean old requests outside the window
        var cutoff = DateTime.UtcNow.Subtract(_options.Window);
        requests!.RemoveAll(r => r < cutoff);

        if (requests.Count >= _options.MaxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for client: {Identifier}. Current requests: {Count}",
                identifier, requests.Count);

            await WriteRateLimitResponse(context);
            return;
        }

        // Add current request
        requests.Add(DateTime.UtcNow);
        _cache.Set(key, requests, _options.Window);        // Add rate limit headers
        context.Response.Headers["X-RateLimit-Limit"] = _options.MaxRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, _options.MaxRequests - requests.Count).ToString();
        context.Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.Add(_options.Window).ToUnixTimeSeconds().ToString();

        await _next(context);
    }

    private static bool ShouldSkipRateLimit(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        return path.Contains("/health") ||
               path.Contains("/swagger") ||
               path.Contains("/metrics");
    }

    private static string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID first, fallback to IP
        var userId = context.User?.FindFirst("sub")?.Value ??
                    context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
            return $"user_{userId}";

        // Fallback to IP address
        var ip = context.Connection.RemoteIpAddress?.ToString() ??
                context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                "unknown";

        return $"ip_{ip}";
    }

    private static async Task WriteRateLimitResponse(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.ContentType = "application/json";

        var response = new
        {
            statusCode = 429,
            message = "Too many requests. Please try again later.",
            retryAfter = "60"
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}

public class RateLimitOptions
{
    public int MaxRequests { get; set; } = 100;
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
}

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, Action<RateLimitOptions>? configure = null)
    {
        var options = new RateLimitOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddMemoryCache();

        return services;
    }

    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitingMiddleware>();
    }
}
