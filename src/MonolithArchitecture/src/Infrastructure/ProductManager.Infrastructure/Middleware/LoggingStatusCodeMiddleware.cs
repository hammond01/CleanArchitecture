using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProductManager.Shared.Logging;
namespace ProductManager.Infrastructure.Middleware;

public class LoggingStatusCodeMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public LoggingStatusCodeMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        var statusCode = context.Response.StatusCode;
        var path = context.Request.Path;
        var method = context.Request.Method;
        var userId = (context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("sub")?.Value) ?? "Anonymous";
        var remoteIp = context.Connection.RemoteIpAddress;

        var statusCodes = new[]
        {
            StatusCodes.Status401Unauthorized, StatusCodes.Status403Forbidden
        };

        if (statusCodes.Contains(statusCode))
        {
            _logger.LogWarning(
            $"StatusCode: {statusCode}, UserId: {ReplaceCRLF(userId)}, Path: {ReplaceCRLF(path)}, Method: {ReplaceCRLF(method)}, IP: {remoteIp}");
        }
    }

    private static string ReplaceCRLF(string text) => text.Replace("\r", "\\r").Replace("\n", "\\n");
}
