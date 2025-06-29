using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.RateLimit;

/// <summary>
/// Rate limiting middleware
/// </summary>
public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitMiddleware> _logger;
    private readonly RateLimitOptions _options;

    public RateLimitMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        ILogger<RateLimitMiddleware> logger,
        RateLimitOptions options)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.EnableRateLimit)
        {
            await _next(context);
            return;
        }

        var clientId = GetClientIdentifier(context);
        var key = $"rate_limit_{clientId}";

        var requestCount = _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _options.Window;
            return 0;
        });

        if (requestCount >= _options.MaxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientId);
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers["Retry-After"] = _options.Window.TotalSeconds.ToString();
            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            return;
        }

        _cache.Set(key, requestCount + 1, _options.Window);

        // Add rate limit headers
        context.Response.Headers["X-RateLimit-Limit"] = _options.MaxRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = (Math.Max(0, _options.MaxRequests - requestCount - 1)).ToString();
        context.Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.Add(_options.Window).ToUnixTimeSeconds().ToString();

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID first, fall back to IP address
        var userId = context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
            return userId;

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        return !string.IsNullOrEmpty(forwarded) ? forwarded : ipAddress ?? "unknown";
    }
}

/// <summary>
/// Rate limiting configuration
/// </summary>
public class RateLimitOptions
{
    public bool EnableRateLimit { get; set; } = true;
    public int MaxRequests { get; set; } = 100;
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
}

/// <summary>
/// Rate limiting attribute for controllers/actions
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RateLimitAttribute : Attribute
{
    public int MaxRequests { get; set; } = 10;
    public int WindowSeconds { get; set; } = 60;
    public string? Policy { get; set; }
}

/// <summary>
/// Extensions for rate limiting
/// </summary>
public static class RateLimitExtensions
{
    public static IServiceCollection AddRateLimit(
        this IServiceCollection services,
        Action<RateLimitOptions>? configure = null)
    {
        var options = new RateLimitOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddMemoryCache();

        return services;
    }

    public static IApplicationBuilder UseRateLimit(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitMiddleware>();
    }
}
