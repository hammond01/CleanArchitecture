using ApiGateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiGateway.Controllers;

/// <summary>
/// Orders controller for API Gateway - routes to Order Management service
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IServiceProxyService _serviceProxy;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IServiceProxyService serviceProxy, ILogger<OrdersController> logger)
    {
        _serviceProxy = serviceProxy;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? customerId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation("Forwarding GetOrders request to Order Management service");

            var queryParams = new List<string>();
            queryParams.Add($"pageNumber={pageNumber}");
            queryParams.Add($"pageSize={pageSize}");
            
            if (!string.IsNullOrEmpty(status))
                queryParams.Add($"status={status}");
            
            if (!string.IsNullOrEmpty(customerId))
                queryParams.Add($"customerId={customerId}");
            
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = string.Join("&", queryParams);
            var path = $"api/v1/OrderManagement?{queryString}";

            var response = await _serviceProxy.ForwardRequestAsync("OrderManagement", path, HttpMethod.Get);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding GetOrders request");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(string orderId)
    {
        try
        {
            _logger.LogInformation("Forwarding GetOrder request for {OrderId}", orderId);

            var path = $"api/v1/OrderManagement/{orderId}";
            var response = await _serviceProxy.ForwardRequestAsync("OrderManagement", path, HttpMethod.Get);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding GetOrder request for {OrderId}", orderId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] object orderData)
    {
        try
        {
            _logger.LogInformation("Forwarding CreateOrder request");

            var path = "api/v1/OrderManagement";
            var response = await _serviceProxy.ForwardRequestAsync("OrderManagement", path, HttpMethod.Post, orderData);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding CreateOrder request");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing order
    /// </summary>
    [HttpPut("{orderId}")]
    public async Task<IActionResult> UpdateOrder(string orderId, [FromBody] object orderData)
    {
        try
        {
            _logger.LogInformation("Forwarding UpdateOrder request for {OrderId}", orderId);

            var path = $"api/v1/OrderManagement/{orderId}";
            var response = await _serviceProxy.ForwardRequestAsync("OrderManagement", path, HttpMethod.Put, orderData);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding UpdateOrder request for {OrderId}", orderId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Confirm an order
    /// </summary>
    [HttpPost("{orderId}/confirm")]
    public async Task<IActionResult> ConfirmOrder(string orderId)
    {
        try
        {
            _logger.LogInformation("Forwarding ConfirmOrder request for {OrderId}", orderId);

            var path = $"api/v1/OrderManagement/{orderId}/confirm";
            var response = await _serviceProxy.ForwardRequestAsync("OrderManagement", path, HttpMethod.Post);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding ConfirmOrder request for {OrderId}", orderId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Cancel an order
    /// </summary>
    [HttpPost("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(string orderId, [FromBody] string reason)
    {
        try
        {
            _logger.LogInformation("Forwarding CancelOrder request for {OrderId}", orderId);

            var path = $"api/v1/OrderManagement/{orderId}/cancel";
            var response = await _serviceProxy.ForwardRequestAsync("OrderManagement", path, HttpMethod.Post, reason);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding CancelOrder request for {OrderId}", orderId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Ship an order
    /// </summary>
    [HttpPost("{orderId}/ship")]
    public async Task<IActionResult> ShipOrder(string orderId, [FromBody] string trackingNumber)
    {
        try
        {
            _logger.LogInformation("Forwarding ShipOrder request for {OrderId}", orderId);

            var path = $"api/v1/OrderManagement/{orderId}/ship";
            var response = await _serviceProxy.ForwardRequestAsync("OrderManagement", path, HttpMethod.Post, trackingNumber);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding ShipOrder request for {OrderId}", orderId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Mark order as delivered
    /// </summary>
    [HttpPost("{orderId}/deliver")]
    public async Task<IActionResult> DeliverOrder(string orderId)
    {
        try
        {
            _logger.LogInformation("Forwarding DeliverOrder request for {OrderId}", orderId);

            var path = $"api/v1/OrderManagement/{orderId}/deliver";
            var response = await _serviceProxy.ForwardRequestAsync("OrderManagement", path, HttpMethod.Post);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding DeliverOrder request for {OrderId}", orderId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get order statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetOrderStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation("Forwarding GetOrderStatistics request");

            var queryParams = new List<string>();
            
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var path = $"api/v1/OrderManagement/statistics{queryString}";

            var response = await _serviceProxy.ForwardRequestAsync("OrderManagement", path, HttpMethod.Get);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding GetOrderStatistics request");
            return StatusCode(500, "Internal server error");
        }
    }
}
