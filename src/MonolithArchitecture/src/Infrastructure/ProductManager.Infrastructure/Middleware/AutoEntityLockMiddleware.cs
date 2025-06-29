using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Identity;
using ProductManager.Shared.Locks;

namespace ProductManager.Infrastructure.Middleware;

/// <summary>
/// Middleware that automatically applies distributed locking for PUT/DELETE operations
/// </summary>
public class AutoEntityLockMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AutoEntityLockMiddleware> _logger;

    // Pattern to match RESTful routes like /v1/Product/{id}, /v1/Category/{id}, etc.
    private static readonly Regex EntityRoutePattern = new(@"^/v1/(?<entityName>\w+)/(?<entityId>[^/]+)/?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public AutoEntityLockMiddleware(RequestDelegate next, ILogger<AutoEntityLockMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only apply locking for PUT/DELETE operations
        if (context.Request.Method != HttpMethods.Put && context.Request.Method != HttpMethods.Delete)
        {
            await _next(context);
            return;
        }

        // Parse the route to extract entity name and ID
        var match = EntityRoutePattern.Match(context.Request.Path.Value ?? "");
        if (!match.Success)
        {
            await _next(context);
            return;
        }

        var entityName = match.Groups["entityName"].Value;
        var entityId = match.Groups["entityId"].Value;

        // Skip if entity name is not a valid entity (e.g., "swagger", "health")
        if (IsSystemRoute(entityName))
        {
            await _next(context);
            return;
        }

        var lockManager = context.RequestServices.GetRequiredService<ILockManager>();
        var currentUser = context.RequestServices.GetRequiredService<ICurrentUser>();

        var ownerId = currentUser.IsAuthenticated ? currentUser.UserId : $"anonymous-{context.Connection.Id}";
        var lockTimeout = TimeSpan.FromMinutes(5); // 5 minutes default timeout

        _logger.LogDebug("Attempting to acquire lock for {EntityName} {EntityId} by user {OwnerId}", entityName, entityId, ownerId);

        // Try to acquire lock
        var lockAcquired = lockManager.AcquireLock(entityName, entityId, ownerId, lockTimeout);

        if (!lockAcquired)
        {
            _logger.LogWarning("Failed to acquire lock for {EntityName} {EntityId} by user {OwnerId}", entityName, entityId, ownerId);

            await WriteConflictResponse(context, entityName, entityId);
            return;
        }

        _logger.LogDebug("Successfully acquired lock for {EntityName} {EntityId} by user {OwnerId}", entityName, entityId, ownerId);

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing request for {EntityName} {EntityId}", entityName, entityId);
            throw;
        }
        finally
        {
            // Always release the lock
            lockManager.ReleaseLock(entityName, entityId, ownerId);
            _logger.LogDebug("Released lock for {EntityName} {EntityId} by user {OwnerId}", entityName, entityId, ownerId);
        }
    }

    private static bool IsSystemRoute(string entityName)
    {
        var systemRoutes = new[] { "swagger", "health", "logs", "identity", "api" };
        return systemRoutes.Contains(entityName.ToLowerInvariant());
    }

    private static async Task WriteConflictResponse(HttpContext context, string entityName, string entityId)
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        context.Response.ContentType = "application/json";

        var response = new
        {
            statusCode = 409,
            isSuccessStatusCode = false,
            message = $"{entityName} is currently being edited by another user. Please try again later.",
            result = new
            {
                entityName,
                entityId,
                conflictType = "EntityLocked"
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
