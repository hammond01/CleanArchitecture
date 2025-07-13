using Microsoft.AspNetCore.Mvc;
using Shared.Common.Constants;
using Shared.Contracts;

namespace ApiGateway.Controllers;

/// <summary>
/// API Gateway controller for routing requests to microservices
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GatewayController> _logger;
    private readonly IConfiguration _configuration;

    public GatewayController(
        IHttpClientFactory httpClientFactory,
        ILogger<GatewayController> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(ApiResponse.Success("API Gateway is healthy"));
    }

    /// <summary>
    /// Get service status
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> GetServiceStatus()
    {
        var services = new[]
        {
            ServiceConstants.ServiceNames.ProductCatalogService,
            ServiceConstants.ServiceNames.CustomerManagementService,
            ServiceConstants.ServiceNames.OrderManagementService
        };

        var serviceStatuses = new Dictionary<string, object>();

        foreach (var service in services)
        {
            try
            {
                var baseUrl = _configuration[$"Services:{service}:BaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    serviceStatuses[service] = new { Status = "Configuration Missing", Healthy = false };
                    continue;
                }

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"{baseUrl}/health");

                serviceStatuses[service] = new
                {
                    Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                    Healthy = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking health for service {ServiceName}", service);
                serviceStatuses[service] = new { Status = "Error", Healthy = false, Error = ex.Message };
            }
        }

        return Ok(ApiResponse<Dictionary<string, object>>.Success(serviceStatuses, "Service status retrieved"));
    }
}
