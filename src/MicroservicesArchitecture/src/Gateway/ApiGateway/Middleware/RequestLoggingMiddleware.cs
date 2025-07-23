using System.Diagnostics;
using System.Text;

namespace ApiGateway.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        // Add request ID to response headers
        context.Response.Headers.Add("X-Request-ID", requestId);
        
        // Log request
        await LogRequestAsync(context, requestId);
        
        // Capture response
        var originalResponseBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;
        
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred for request {RequestId}", requestId);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            
            // Log response
            await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);
            
            // Copy response back to original stream
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        try
        {
            var request = context.Request;
            var requestBody = string.Empty;

            // Read request body for POST/PUT requests
            if (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH")
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            _logger.LogInformation(
                "HTTP Request {RequestId}: {Method} {Path}{QueryString} from {RemoteIpAddress} | Body: {RequestBody}",
                requestId,
                request.Method,
                request.Path,
                request.QueryString,
                context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                string.IsNullOrEmpty(requestBody) ? "Empty" : TruncateString(requestBody, 500)
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log request for {RequestId}", requestId);
        }
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMilliseconds)
    {
        try
        {
            var response = context.Response;
            var responseBody = string.Empty;

            // Read response body
            if (response.Body.CanSeek)
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
                responseBody = await reader.ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
            }

            var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
            
            _logger.Log(logLevel,
                "HTTP Response {RequestId}: {StatusCode} in {ElapsedMilliseconds}ms | Body: {ResponseBody}",
                requestId,
                response.StatusCode,
                elapsedMilliseconds,
                string.IsNullOrEmpty(responseBody) ? "Empty" : TruncateString(responseBody, 500)
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log response for {RequestId}", requestId);
        }
    }

    private static string TruncateString(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;

        return value[..maxLength] + "...";
    }
}

/// <summary>
/// Extension method to register the middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
