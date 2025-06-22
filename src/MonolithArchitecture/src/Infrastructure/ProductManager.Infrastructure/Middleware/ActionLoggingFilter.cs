using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Common;
namespace ProductManager.Infrastructure.Middleware;

/// <summary>
///     Action filter to log API actions when decorated with LogActionAttribute
/// </summary>
public class ActionLoggingFilter : ActionFilterAttribute
{
    private readonly IActionLogService _actionLogService;
    private readonly ILogger<ActionLoggingFilter> _logger;

    public ActionLoggingFilter(IActionLogService actionLogService, ILogger<ActionLoggingFilter> logger)
    {
        _actionLogService = actionLogService;
        _logger = logger;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logActionAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<LogActionAttribute>()
            .FirstOrDefault();

        if (logActionAttribute == null)
        {
            await next();
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var actionName = $"{context.ActionDescriptor.RouteValues["controller"]}.{context.ActionDescriptor.RouteValues["action"]}";
        var userId = GetUserId(context.HttpContext);
        var clientIp = GetClientIpAddress(context.HttpContext);
        var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();

        _logger.LogInformation("🎬 Starting action: {ActionName} for user: {UserId}", actionName, userId ?? "Anonymous");

        ActionExecutedContext? executedContext = null;
        Exception? exception = null;

        try
        {
            executedContext = await next();
            exception = executedContext.Exception;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            var isSuccess = exception == null && executedContext?.Exception == null;
            var statusCode = executedContext?.HttpContext.Response.StatusCode ?? 500;

            try
            {
                await _actionLogService.LogActionAsync(new ActionLogRequest
                {
                    ActionName = actionName,
                    Description = logActionAttribute.Description,
                    UserId = userId,
                    ClientIpAddress = clientIp,
                    UserAgent = userAgent,
                    ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds,
                    IsSuccess = isSuccess,
                    ErrorMessage = exception?.Message,
                    StatusCode = statusCode,
                    RequestParameters = GetRequestParameters(context),
                    ResponseData = isSuccess ? GetResponseData(executedContext) : null
                });

                _logger.LogInformation("🎭 Completed action: {ActionName} in {ElapsedMs}ms - Success: {IsSuccess}",
                actionName, stopwatch.ElapsedMilliseconds, isSuccess);
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "❌ Failed to log action: {ActionName}", actionName);
            }
        }
    }

    private static string? GetUserId(HttpContext httpContext) => httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                                                 httpContext.User.FindFirst("sub")?.Value ??
                                                                 httpContext.User.FindFirst("userId")?.Value;

    private static string GetClientIpAddress(HttpContext httpContext) => httpContext.Connection.RemoteIpAddress?.ToString() ??
                                                                         httpContext.Request.Headers["X-Forwarded-For"]
                                                                             .FirstOrDefault() ??
                                                                         httpContext.Request.Headers["X-Real-IP"].FirstOrDefault() ??
                                                                         "Unknown";

    private static string? GetRequestParameters(ActionExecutingContext context)
    {
        try
        {
            if (context.ActionArguments.Count == 0)
            {
                return null;
            }

            var parameters = new Dictionary<string, object?>();
            foreach (var arg in context.ActionArguments)
            {
                // Avoid logging sensitive data
                if (IsSensitiveParameter(arg.Key))
                {
                    parameters[arg.Key] = "***HIDDEN***";
                }
                else
                {
                    parameters[arg.Key] = arg.Value;
                }
            }

            return JsonSerializer.Serialize(parameters, new JsonSerializerOptions
            {
                WriteIndented = false, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
        catch
        {
            return null;
        }
    }

    private static string? GetResponseData(ActionExecutedContext? context)
    {
        try
        {
            if (context?.Result is ObjectResult objectResult)
            {
                return JsonSerializer.Serialize(objectResult.Value, new JsonSerializerOptions
                {
                    WriteIndented = false, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsSensitiveParameter(string parameterName)
    {
        var sensitiveWords = new[]
        {
            "password", "token", "secret", "key", "auth", "credential"
        };
        return sensitiveWords.Any(word => parameterName.ToLowerInvariant().Contains(word));
    }
}
