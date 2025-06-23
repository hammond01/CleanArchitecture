using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Identity;
using ProductManager.Shared.Locks;
using System.Net;

namespace ProductManager.Infrastructure.Middleware;

/// <summary>
/// Attribute to apply distributed locking on controller actions
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class EntityLockAttribute : ActionFilterAttribute
{
    public string EntityName { get; }
    public string EntityIdParameterName { get; }
    public int LockTimeoutMinutes { get; }
    public bool ThrowOnLockFailure { get; }

    /// <summary>
    /// Apply distributed lock on entity
    /// </summary>
    /// <param name="entityName">Name of entity type (e.g., "Product", "Category")</param>
    /// <param name="entityIdParameterName">Name of parameter containing entity ID (e.g., "id")</param>
    /// <param name="lockTimeoutMinutes">Lock timeout in minutes (default: 5)</param>
    /// <param name="throwOnLockFailure">Throw exception if lock fails (default: false, returns 409 Conflict)</param>
    public EntityLockAttribute(string entityName, string entityIdParameterName = "id", int lockTimeoutMinutes = 5, bool throwOnLockFailure = false)
    {
        EntityName = entityName;
        EntityIdParameterName = entityIdParameterName;
        LockTimeoutMinutes = lockTimeoutMinutes;
        ThrowOnLockFailure = throwOnLockFailure;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var lockManager = context.HttpContext.RequestServices.GetRequiredService<ILockManager>();
        var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();

        // Get entity ID from action parameters
        if (!context.ActionArguments.TryGetValue(EntityIdParameterName, out var entityIdObj) || entityIdObj == null)
        {
            context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
            {
                error = $"Parameter '{EntityIdParameterName}' is required for entity locking"
            });
            return;
        }

        var entityId = entityIdObj.ToString()!;
        var ownerId = currentUser.IsAuthenticated ? currentUser.UserId : "anonymous";
        var lockTimeout = TimeSpan.FromMinutes(LockTimeoutMinutes);

        // Try to acquire lock
        var lockAcquired = lockManager.AcquireLock(EntityName, entityId, ownerId, lockTimeout);

        if (!lockAcquired)
        {
            if (ThrowOnLockFailure)
            {
                throw new InvalidOperationException($"Could not acquire lock for {EntityName} with ID {entityId}. Entity may be locked by another user.");
            }

            context.Result = new Microsoft.AspNetCore.Mvc.ObjectResult(new
            {
                statusCode = (int)HttpStatusCode.Conflict,
                message = $"{EntityName} is currently being edited by another user. Please try again later.",
                entityName = EntityName,
                entityId = entityId
            })
            {
                StatusCode = (int)HttpStatusCode.Conflict
            };
            return;
        }

        try
        {
            // Execute the action
            var result = await next();

            // If action failed, we still want to release the lock
            if (result.Exception != null && !result.ExceptionHandled)
            {
                // Log the exception but don't rethrow yet
                var logger = context.HttpContext.RequestServices.GetService<ILogger<EntityLockAttribute>>();
                logger?.LogError(result.Exception, "Action failed while holding lock for {EntityName} {EntityId}", EntityName, entityId);
            }
        }
        finally
        {
            // Always release lock
            lockManager.ReleaseLock(EntityName, entityId, ownerId);
        }
    }
}
