using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Infrastructure.Middleware;
using ProductManager.Shared.Locks;
namespace ProductManager.Api.Controllers;

/// <summary>
///     Test controller to demonstrate distributed locking functionality
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lock-tests")]
public class LockTestController : ControllerBase
{
    private readonly ILockManager _lockManager;
    private readonly ILogger<LockTestController> _logger;

    public LockTestController(ILockManager lockManager, ILogger<LockTestController> logger)
    {
        _lockManager = lockManager;
        _logger = logger;
    }
    /// <summary>
    ///     Test explicit locking using EntityLockAttribute
    /// </summary>
    [HttpPost("explicit/{resourceId}")]
    [EntityLock("TestResource", "resourceId", 30)]
    public async Task<IActionResult> TestExplicitLock(string resourceId, [FromBody] string data)
    {
        _logger.LogInformation("🔒 Testing explicit lock for resource: {ResourceId}", resourceId);

        // Simulate work that needs to be protected by lock
        await Task.Delay(2000);

        _logger.LogInformation("✅ Explicit lock test completed for resource: {ResourceId}", resourceId);
        return Ok(new
        {
            ResourceId = resourceId,
            Data = data,
            Message = "Explicit lock test completed"
        });
    }

    /// <summary>
    ///     Test automatic locking via AutoEntityLockMiddleware (PUT operation)
    /// </summary>
    [HttpPut("automatic/{resourceId}")]
    public async Task<IActionResult> TestAutomaticLockPut(string resourceId, [FromBody] string data)
    {
        _logger.LogInformation("🔄 Testing automatic lock (PUT) for resource: {ResourceId}", resourceId);

        // Simulate work that needs to be protected by lock
        await Task.Delay(2000);

        _logger.LogInformation("✅ Automatic lock (PUT) test completed for resource: {ResourceId}", resourceId);
        return Ok(new
        {
            ResourceId = resourceId,
            Data = data,
            Message = "Automatic PUT lock test completed"
        });
    }

    /// <summary>
    ///     Test automatic locking via AutoEntityLockMiddleware (DELETE operation)
    /// </summary>
    [HttpDelete("automatic/{resourceId}")]
    public async Task<IActionResult> TestAutomaticLockDelete(string resourceId)
    {
        _logger.LogInformation("🗑️ Testing automatic lock (DELETE) for resource: {ResourceId}", resourceId);

        // Simulate work that needs to be protected by lock
        await Task.Delay(2000);

        _logger.LogInformation("✅ Automatic lock (DELETE) test completed for resource: {ResourceId}", resourceId);
        return Ok(new
        {
            ResourceId = resourceId,
            Message = "Automatic DELETE lock test completed"
        });
    }

    /// <summary>
    ///     Test manual lock acquisition and release
    /// </summary>
    [HttpPost("manual/{resourceId}")]
    public async Task<IActionResult> TestManualLock(string resourceId, [FromBody] string data)
    {
        _logger.LogInformation("🔐 Testing manual lock for resource: {ResourceId}", resourceId);

        var ownerId = $"test-{Guid.NewGuid()}";

        try
        {
            // Try to acquire lock
            var lockAcquired = _lockManager.AcquireLock("ManualTestResource", resourceId, ownerId, TimeSpan.FromSeconds(30));

            if (!lockAcquired)
            {
                _logger.LogWarning("⚠️ Could not acquire lock for resource: {ResourceId}", resourceId);
                return Conflict(new
                {
                    ResourceId = resourceId,
                    Message = "Resource is currently locked by another process"
                });
            }

            _logger.LogInformation("🔒 Manual lock acquired for resource: {ResourceId}", resourceId);

            // Simulate work that needs to be protected by lock
            await Task.Delay(2000);

            // Release the lock
            var lockReleased = _lockManager.ReleaseLock("ManualTestResource", resourceId, ownerId);
            _logger.LogInformation("🔓 Manual lock released for resource: {ResourceId}, Success: {Success}", resourceId, lockReleased);

            _logger.LogInformation("✅ Manual lock test completed for resource: {ResourceId}", resourceId);
            return Ok(new
            {
                ResourceId = resourceId,
                Data = data,
                Message = "Manual lock test completed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Manual lock test failed for resource: {ResourceId}", resourceId);

            // Try to release lock in case of error
            try
            {
                _lockManager.ReleaseLock("ManualTestResource", resourceId, ownerId);
            }
            catch
            {
                // ignored
            }// Ignore errors during cleanup

            return StatusCode(500, new
            {
                ResourceId = resourceId,
                Error = ex.Message
            });
        }
    }

    /// <summary>
    ///     Get information about whether a resource is currently locked
    /// </summary>
    [HttpGet("status/{resourceId}")]
    public IActionResult GetLockStatus(string resourceId)
    {
        _logger.LogInformation("ℹ️ Checking lock status for resource: {ResourceId}", resourceId);

        var ownerId = $"status-check-{Guid.NewGuid()}";

        try
        {
            // Try to acquire a very short lock to test if the resource is available
            var lockAcquired = _lockManager.AcquireLock("StatusCheck", resourceId, ownerId, TimeSpan.FromMilliseconds(100));

            if (lockAcquired)
            {
                // Release immediately since this was just a test
                _lockManager.ReleaseLock("StatusCheck", resourceId, ownerId);
                return Ok(new
                {
                    ResourceId = resourceId,
                    IsLocked = false,
                    Message = "Resource is available"
                });
            }
            return Ok(new
            {
                ResourceId = resourceId,
                IsLocked = true,
                Message = "Resource is currently locked"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error checking lock status for resource: {ResourceId}", resourceId);
            return StatusCode(500, new
            {
                ResourceId = resourceId,
                Error = ex.Message
            });
        }
    }
}
