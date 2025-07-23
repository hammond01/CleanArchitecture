using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiGateway.Controllers;

/// <summary>
/// Health check controller for monitoring microservices
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IServiceProxyService _serviceProxy;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        IServiceProxyService serviceProxy, 
        IConfiguration configuration,
        ILogger<HealthController> logger)
    {
        _serviceProxy = serviceProxy;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Get health status of API Gateway
    /// </summary>
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "API Gateway",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }

    /// <summary>
    /// Get health status of all microservices
    /// </summary>
    [HttpGet("services")]
    public async Task<IActionResult> GetServicesHealth()
    {
        var servicesHealth = new List<object>();
        var services = new[]
        {
            ("OrderManagement", "Order Management Service"),
            ("ProductCatalog", "Product Catalog Service"),
            ("CustomerManagement", "Customer Management Service"),
            ("InventoryManagement", "Inventory Management Service"),
            ("ShippingLogistics", "Shipping Logistics Service"),
            ("IdentityAccess", "Identity Access Service")
        };

        foreach (var (serviceName, displayName) in services)
        {
            var healthStatus = await CheckServiceHealthAsync(serviceName, displayName);
            servicesHealth.Add(healthStatus);
        }

        var overallStatus = servicesHealth.All(s => ((dynamic)s).Status == "Healthy") ? "Healthy" : "Degraded";

        return Ok(new
        {
            OverallStatus = overallStatus,
            Timestamp = DateTime.UtcNow,
            Services = servicesHealth
        });
    }

    /// <summary>
    /// Get health status of a specific service
    /// </summary>
    [HttpGet("services/{serviceName}")]
    public async Task<IActionResult> GetServiceHealth(string serviceName)
    {
        try
        {
            var healthStatus = await CheckServiceHealthAsync(serviceName, serviceName);
            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health for service {ServiceName}", serviceName);
            return Ok(new
            {
                Service = serviceName,
                Status = "Unhealthy",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    private async Task<object> CheckServiceHealthAsync(string serviceName, string displayName)
    {
        try
        {
            _logger.LogDebug("Checking health for service {ServiceName}", serviceName);

            var response = await _serviceProxy.ForwardRequestAsync(serviceName, "health", HttpMethod.Get);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var healthData = JsonSerializer.Deserialize<JsonElement>(content);
                
                return new
                {
                    Service = displayName,
                    Status = "Healthy",
                    ResponseTime = $"{response.Headers.Date?.Subtract(DateTime.UtcNow).TotalMilliseconds:F0}ms",
                    Details = healthData,
                    Timestamp = DateTime.UtcNow
                };
            }
            else
            {
                return new
                {
                    Service = displayName,
                    Status = "Unhealthy",
                    StatusCode = (int)response.StatusCode,
                    Error = await response.Content.ReadAsStringAsync(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Service {ServiceName} is unreachable", serviceName);
            return new
            {
                Service = displayName,
                Status = "Unreachable",
                Error = "Service is not responding",
                Details = ex.Message,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Timeout checking health for service {ServiceName}", serviceName);
            return new
            {
                Service = displayName,
                Status = "Timeout",
                Error = "Health check timed out",
                Details = ex.Message,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health for service {ServiceName}", serviceName);
            return new
            {
                Service = displayName,
                Status = "Error",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Get API Gateway metrics and statistics
    /// </summary>
    [HttpGet("metrics")]
    public IActionResult GetMetrics()
    {
        // In a real implementation, you would collect actual metrics
        // This is a simplified example
        return Ok(new
        {
            ApiGateway = new
            {
                Status = "Healthy",
                Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\.hh\:mm\:ss"),
                RequestCount = Random.Shared.Next(1000, 10000),
                ErrorRate = $"{Random.Shared.NextDouble() * 5:F2}%",
                AverageResponseTime = $"{Random.Shared.Next(50, 200)}ms"
            },
            System = new
            {
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                WorkingSet = $"{Environment.WorkingSet / 1024 / 1024} MB",
                GCMemory = $"{GC.GetTotalMemory(false) / 1024 / 1024} MB"
            },
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Ping endpoint for basic connectivity test
    /// </summary>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            Message = "Pong",
            Timestamp = DateTime.UtcNow,
            Server = Environment.MachineName
        });
    }
}
