using ApiGateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiGateway.Controllers;

/// <summary>
/// Products controller for API Gateway - routes to Product Catalog service
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IServiceProxyService _serviceProxy;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IServiceProxyService serviceProxy, ILogger<ProductsController> logger)
    {
        _serviceProxy = serviceProxy;
        _logger = logger;
    }

    /// <summary>
    /// Get all products with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? categoryId = null,
        [FromQuery] string? supplierId = null,
        [FromQuery] bool? discontinued = null,
        [FromQuery] string? search = null)
    {
        try
        {
            _logger.LogInformation("Forwarding GetProducts request to Product Catalog service");

            var queryParams = new List<string>();
            queryParams.Add($"pageNumber={pageNumber}");
            queryParams.Add($"pageSize={pageSize}");
            
            if (!string.IsNullOrEmpty(categoryId))
                queryParams.Add($"categoryId={categoryId}");
            
            if (!string.IsNullOrEmpty(supplierId))
                queryParams.Add($"supplierId={supplierId}");
            
            if (discontinued.HasValue)
                queryParams.Add($"discontinued={discontinued.Value}");
            
            if (!string.IsNullOrEmpty(search))
                queryParams.Add($"search={search}");

            var queryString = string.Join("&", queryParams);
            var path = $"api/products?{queryString}";

            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Get);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding GetProducts request");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProduct(string productId)
    {
        try
        {
            _logger.LogInformation("Forwarding GetProduct request for {ProductId}", productId);

            var path = $"api/products/{productId}";
            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Get);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding GetProduct request for {ProductId}", productId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] object productData)
    {
        try
        {
            _logger.LogInformation("Forwarding CreateProduct request");

            var path = "api/products";
            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Post, productData);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding CreateProduct request");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProduct(string productId, [FromBody] object productData)
    {
        try
        {
            _logger.LogInformation("Forwarding UpdateProduct request for {ProductId}", productId);

            var path = $"api/products/{productId}";
            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Put, productData);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding UpdateProduct request for {ProductId}", productId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(string productId)
    {
        try
        {
            _logger.LogInformation("Forwarding DeleteProduct request for {ProductId}", productId);

            var path = $"api/products/{productId}";
            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Delete);
            
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding DeleteProduct request for {ProductId}", productId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(string categoryId)
    {
        try
        {
            _logger.LogInformation("Forwarding GetProductsByCategory request for {CategoryId}", categoryId);

            var path = $"api/products/category/{categoryId}";
            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Get);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding GetProductsByCategory request for {CategoryId}", categoryId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get products by supplier
    /// </summary>
    [HttpGet("supplier/{supplierId}")]
    public async Task<IActionResult> GetProductsBySupplier(string supplierId)
    {
        try
        {
            _logger.LogInformation("Forwarding GetProductsBySupplier request for {SupplierId}", supplierId);

            var path = $"api/products/supplier/{supplierId}";
            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Get);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding GetProductsBySupplier request for {SupplierId}", supplierId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Search products
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string query, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Forwarding SearchProducts request for query: {Query}", query);

            var queryParams = new List<string>();
            queryParams.Add($"query={query}");
            queryParams.Add($"pageNumber={pageNumber}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = string.Join("&", queryParams);
            var path = $"api/products/search?{queryString}";

            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Get);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding SearchProducts request for query: {Query}", query);
            return StatusCode(500, "Internal server error");
        }
    }
}
