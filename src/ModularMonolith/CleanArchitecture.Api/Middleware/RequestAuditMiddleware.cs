using System.Security.Claims;
using System.Text.Json;
using BuildingBlocks.Application.Auditing;
using Auditing.Domain.Entities;
using Auditing.Domain.Repositories;
using CleanArchitecture.Api.Auditing;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CleanArchitecture.Api.Middleware;

/// <summary>
/// Persists a lightweight audit entry for mutating HTTP requests.
/// </summary>
public class RequestAuditMiddleware
{
    private static readonly HashSet<string> AuditedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete
    };

    private readonly RequestDelegate _next;

    public RequestAuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!AuditedMethods.Contains(context.Request.Method))
        {
            await _next(context);
            return;
        }

        await _next(context);

        try
        {
            var entityChangeBuffer = context.RequestServices.GetService<IEntityChangeBuffer>();
            var entityChanges = entityChangeBuffer?.Drain() ?? Array.Empty<EntityChange>();

            if (entityChanges.Count > 0)
            {
                var auditOutboxProcessor = context.RequestServices.GetService<IAuditOutboxProcessor>();
                if (auditOutboxProcessor != null)
                {
                    await auditOutboxProcessor.ProcessPendingMessagesAsync(100, context.RequestAborted);
                }

                return;
            }

            var auditRepository = context.RequestServices.GetService<IAuditLogRepository>();
            if (auditRepository == null)
            {
                return;
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            var requestPath = context.Request.Path.Value ?? "/";
            var queryString = context.Request.QueryString.HasValue
                ? context.Request.QueryString.Value
                : string.Empty;
            var requestAuditEntry = new AuditLogEntry
            {
                UserId = userId,
                Action = $"{context.Request.Method} {requestPath}",
                ObjectId = context.TraceIdentifier,
                Log = JsonSerializer.Serialize(new
                {
                    method = context.Request.Method,
                    path = requestPath,
                    queryString,
                    statusCode = context.Response.StatusCode
                }),
                CreatedDateTime = DateTimeOffset.UtcNow
            };

            await auditRepository.CreateEntryAsync(requestAuditEntry, context.RequestAborted);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to persist request audit entry");
        }
    }
}

public static class RequestAuditMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuditMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestAuditMiddleware>();
    }
}
