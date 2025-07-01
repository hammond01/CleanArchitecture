using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Common;
using ProductManager.Persistence;

namespace ProductManager.Api.Controllers;

/// <summary>
/// Controller for system health checks and monitoring
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/health")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    [HttpGet]
    [LogAction("Health check")]
    public ActionResult<object> GetHealth()
    {
        _logger.LogInformation("🏥 Health check requested");

        var result = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        };

        return Ok(result);
    }

    /// <summary>
    /// Detailed health check with database connectivity
    /// </summary>
    [HttpGet("detailed")]
    [LogAction("Detailed health check")]
    public async Task<ActionResult<object>> GetDetailedHealth()
    {
        _logger.LogInformation("🏥 Detailed health check requested");

        var healthData = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            Database = await CheckDatabaseHealth(),
            System = GetSystemInfo()
        };

        return Ok(healthData);
    }

    /// <summary>
    /// Get database statistics
    /// </summary>
    [HttpGet("database")]
    [LogAction("Database health check")]
    public async Task<ActionResult<object>> GetDatabaseHealth()
    {
        _logger.LogInformation("🏥 Database health check requested");

        var dbHealth = await CheckDatabaseHealth();
        return Ok(dbHealth);
    }

    /// <summary>
    /// Get system information
    /// </summary>
    [HttpGet("system")]
    [LogAction("System info check")]
    public ActionResult<object> GetSystemInfo()
    {
        _logger.LogInformation("🏥 System info requested");

        var systemInfo = new
        {
            MachineName = Environment.MachineName,
            OSVersion = Environment.OSVersion.ToString(),
            ProcessorCount = Environment.ProcessorCount,
            WorkingSet = GC.GetTotalMemory(false),
            FrameworkVersion = Environment.Version.ToString(),
            CurrentDirectory = Environment.CurrentDirectory,
            SystemUptime = TimeSpan.FromMilliseconds(Environment.TickCount64)
        };

        return Ok(systemInfo);
    }

    private async Task<object> CheckDatabaseHealth()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
            {
                return new
                {
                    Status = "Unhealthy",
                    Message = "Cannot connect to database",
                    ConnectionString = _context.Database.GetConnectionString()?.Split(';')[0] // Only show server part
                };
            }

            // Get table counts
            var productCount = await _context.Products.CountAsync();
            var categoryCount = await _context.Categories.CountAsync();
            var orderCount = await _context.Orders.CountAsync();
            var customerCount = await _context.Customers.CountAsync();
            var employeeCount = await _context.Employees.CountAsync();
            var supplierCount = await _context.Suppliers.CountAsync();

            return new
            {
                Status = "Healthy",
                Message = "Database connection successful",
                Statistics = new
                {
                    Products = productCount,
                    Categories = categoryCount,
                    Orders = orderCount,
                    Customers = customerCount,
                    Employees = employeeCount,
                    Suppliers = supplierCount
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Database health check failed");
            return new
            {
                Status = "Unhealthy",
                Message = "Database health check failed",
                Error = ex.Message
            };
        }
    }
}
