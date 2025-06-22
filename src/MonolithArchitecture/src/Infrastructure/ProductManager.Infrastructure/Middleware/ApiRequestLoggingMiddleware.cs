using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Common;

namespace ProductManager.Infrastructure.Middleware;

public class ApiRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiRequestLoggingMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ApiRequestLoggingMiddleware(RequestDelegate next, ILogger<ApiRequestLoggingMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        // Skip logging for static files, health checks, etc.
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var originalBodyStream = context.Response.Body;

        using var requestBodyStream = new MemoryStream();
        using var responseBodyStream = new MemoryStream();

        // Capture request body
        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        // Capture response body
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Read request body
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            var requestBody = await new StreamReader(requestBodyStream).ReadToEndAsync();

            // Read response body
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            // Copy response back to original stream
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);

            // Log to database asynchronously with new scope (fire and forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var actionLogService = scope.ServiceProvider.GetRequiredService<IActionLogService>();

                    await actionLogService.LogApiRequestAsync(
                        context.Request.Method,
                        context.Request.Path.Value ?? "",
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds,
                        GetUserId(context)?.ToString(),
                        GetClientIpAddress(context),
                        context.Request.Headers["User-Agent"].FirstOrDefault(),
                        null, // requestSize
                        null  // responseSize
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to log API request to database");
                }
            });

            // Log to console/file
            _logger.LogInformation("🌐 {Method} {Path} → {StatusCode} ({ElapsedMs}ms) | IP: {ClientIP}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                GetClientIpAddress(context));
        }
    }

    private static bool ShouldSkipLogging(PathString path)
    {
        var pathValue = path.Value?.ToLower() ?? "";

        return pathValue.Contains("/swagger") ||
               pathValue.Contains("/health") ||
               pathValue.Contains("/_vs/") ||
               pathValue.Contains("/favicon.ico") ||
               pathValue.Contains(".css") ||
               pathValue.Contains(".js") ||
               pathValue.Contains(".map") ||
               pathValue.Contains("/assets/");
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // Try to get IP from X-Forwarded-For header (if behind proxy)
        var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            return xForwardedFor.Split(',')[0].Trim();
        }

        // Try X-Real-IP header
        var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp;
        }

        // Fallback to remote IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private static Guid? GetUserId(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
        }
        return null;
    }

    private static string TruncateString(string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
            return input;

        return input.Substring(0, maxLength) + "... [TRUNCATED]";
    }
}
