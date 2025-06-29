// Comprehensive Health Checks
using System.Data;
using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Repositories;

namespace ProductManager.Infrastructure.HealthChecks;

/// <summary>
/// Advanced health check for database connectivity and performance
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(IUnitOfWork unitOfWork, ILogger<DatabaseHealthCheck> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();            // Test basic connectivity
            using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            stopwatch.Stop();

            var responseTime = stopwatch.ElapsedMilliseconds;
            var data = new Dictionary<string, object>
            {
                {"responseTime", $"{responseTime}ms"},
                {"timestamp", DateTime.UtcNow},
                {"server", Environment.MachineName}
            };

            if (responseTime > 5000) // 5 seconds threshold
            {
                return HealthCheckResult.Degraded(
                    $"Database response time is slow: {responseTime}ms",
                    data: data);
            }

            if (responseTime > 1000) // 1 second warning
            {
                _logger.LogWarning("Database response time is elevated: {ResponseTime}ms", responseTime);
            }

            return HealthCheckResult.Healthy(
                $"Database is responsive: {responseTime}ms",
                data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy(
                $"Database connection failed: {ex.Message}",
                ex);
        }
    }
}

/// <summary>
/// Application performance health check
/// </summary>
public class ApplicationHealthCheck : IHealthCheck
{
    private readonly ILogger<ApplicationHealthCheck> _logger;

    public ApplicationHealthCheck(ILogger<ApplicationHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var memoryUsage = process.WorkingSet64 / 1024 / 1024; // MB
            var cpuTime = process.TotalProcessorTime;

            var data = new Dictionary<string, object>
            {
                {"memoryUsageMB", memoryUsage},
                {"cpuTime", cpuTime.ToString()},
                {"startTime", process.StartTime},
                {"uptime", DateTime.UtcNow - process.StartTime},
                {"threadCount", process.Threads.Count},
                {"gcMemory", GC.GetTotalMemory(false) / 1024 / 1024} // MB
            };

            // Memory threshold checks
            if (memoryUsage > 1000) // 1GB
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"High memory usage: {memoryUsage}MB",
                    data: data));
            }

            if (memoryUsage > 500) // 500MB warning
            {
                _logger.LogWarning("Elevated memory usage: {MemoryUsage}MB", memoryUsage);
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                $"Application is healthy. Memory: {memoryUsage}MB",
                data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                $"Application health check failed: {ex.Message}",
                ex));
        }
    }
}
