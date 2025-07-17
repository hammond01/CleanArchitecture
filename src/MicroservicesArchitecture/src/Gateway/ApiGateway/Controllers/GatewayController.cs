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

    /// <summary>
    /// Forward requests to OrderManagement service
    /// </summary>
    [HttpGet("orders")]
    [HttpPost("orders")]
    [HttpPut("orders/{id}")]
    [HttpDelete("orders/{id}")]
    [HttpPatch("orders/{id}/status")]
    public async Task<IActionResult> ForwardToOrderManagement()
    {
        var client = _httpClientFactory.CreateClient();
        var orderServiceUrl = _configuration.GetValue<string>("Services:OrderManagement:BaseUrl") ?? "https://localhost:5003";

        // Get the remaining path after /api/gateway/
        var path = HttpContext.Request.Path.Value?.Replace("/api/gateway/", "");
        var queryString = HttpContext.Request.QueryString.Value;

        var targetUrl = $"{orderServiceUrl}/api/{path}{queryString}";

        try
        {
            var request = new HttpRequestMessage(
                new HttpMethod(HttpContext.Request.Method),
                targetUrl);

            // Copy headers
            foreach (var header in HttpContext.Request.Headers)
            {
                if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Copy body for POST/PUT requests
            if (HttpContext.Request.Method == "POST" || HttpContext.Request.Method == "PUT" || HttpContext.Request.Method == "PATCH")
            {
                var body = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            }

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = responseContent,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                StatusCode = (int)response.StatusCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding request to OrderManagement service");
            return StatusCode(503, ApiResponse.Failure("OrderManagement service unavailable"));
        }
    }

    /// <summary>
    /// Forward requests to OrderManagement customers endpoint
    /// </summary>
    [HttpGet("customers")]
    [HttpPost("customers")]
    [HttpPut("customers/{id}")]
    [HttpDelete("customers/{id}")]
    public async Task<IActionResult> ForwardToCustomerManagement()
    {
        var client = _httpClientFactory.CreateClient();
        var orderServiceUrl = _configuration.GetValue<string>("Services:OrderManagement:BaseUrl") ?? "https://localhost:5003";

        // Get the remaining path after /api/gateway/
        var path = HttpContext.Request.Path.Value?.Replace("/api/gateway/", "");
        var queryString = HttpContext.Request.QueryString.Value;

        var targetUrl = $"{orderServiceUrl}/api/{path}{queryString}";

        try
        {
            var request = new HttpRequestMessage(
                new HttpMethod(HttpContext.Request.Method),
                targetUrl);

            // Copy headers
            foreach (var header in HttpContext.Request.Headers)
            {
                if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Copy body for POST/PUT requests
            if (HttpContext.Request.Method == "POST" || HttpContext.Request.Method == "PUT")
            {
                var body = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            }

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = responseContent,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                StatusCode = (int)response.StatusCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding request to OrderManagement service");
            return StatusCode(503, ApiResponse.Failure("OrderManagement service unavailable"));
        }
    }
}
