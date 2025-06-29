// Background services for maintenance and cleanup
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ProductManager.Persistence.BackgroundServices;

/// <summary>
/// Background service for cleaning up old audit logs and maintaining database health
/// </summary>
public class DatabaseMaintenanceService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DatabaseMaintenanceService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Run daily

    public DatabaseMaintenanceService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DatabaseMaintenanceService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Database Maintenance Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformMaintenance();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during database maintenance");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("🛑 Database Maintenance Service stopped");
    }
    private async Task PerformMaintenance()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _logger.LogInformation("🧹 Starting database maintenance tasks");

        try
        {
            // Clean up old audit logs (older than 90 days)
            var cutoffDate = DateTime.UtcNow.AddDays(-90);
            var oldAuditLogs = context.AuditLogs.Where(a => a.CreatedDateTime < cutoffDate);
            var auditCount = oldAuditLogs.Count();

            if (auditCount > 0)
            {
                context.AuditLogs.RemoveRange(oldAuditLogs);
                _logger.LogInformation("🗑️ Removing {Count} old audit log entries", auditCount);
            }            // Clean up old API logs (older than 30 days)
            var oldApiLogs = context.ApiLogs.Where(l => l.RequestTime < DateTime.UtcNow.AddDays(-30));
            var apiLogCount = oldApiLogs.Count();

            if (apiLogCount > 0)
            {
                context.ApiLogs.RemoveRange(oldApiLogs);
                _logger.LogInformation("� Removing {Count} old API log entries", apiLogCount);
            }

            var changesCount = await context.SaveChangesAsync();
            if (changesCount > 0)
            {
                _logger.LogInformation("✅ Database maintenance completed. {Changes} records affected", changesCount);
            }
            else
            {
                _logger.LogInformation("✅ Database maintenance completed. No changes needed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Database maintenance failed");
            throw;
        }
    }
}

/// <summary>
/// Background service for processing periodic tasks (simplified example)
/// </summary>
public class PeriodicTaskProcessorService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<PeriodicTaskProcessorService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Process every 5 minutes

    public PeriodicTaskProcessorService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<PeriodicTaskProcessorService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Periodic Task Processor Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPeriodicTasks();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing periodic tasks");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("🛑 Periodic Task Processor Service stopped");
    }

    private async Task ProcessPeriodicTasks()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _logger.LogDebug("� Processing periodic tasks");

        try
        {
            // Example: Update statistics, send notifications, etc.
            var productCount = await context.Products.CountAsync();
            var customerCount = await context.Customers.CountAsync();
            var orderCount = await context.Orders.CountAsync();

            _logger.LogDebug("📊 Current statistics - Products: {ProductCount}, Customers: {CustomerCount}, Orders: {OrderCount}",
                productCount, customerCount, orderCount);

            // Here you could implement:
            // - Send periodic health reports
            // - Update cached statistics
            // - Send summary emails
            // - Clean up temporary files
            // - Process scheduled tasks

            _logger.LogDebug("✅ Periodic tasks completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during periodic task processing");
            throw;
        }
    }
}
