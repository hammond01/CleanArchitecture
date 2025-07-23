using ApiGateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiGateway.Controllers;

/// <summary>
/// Categories controller for API Gateway - routes to Product Catalog service
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IServiceProxyService _serviceProxy;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IServiceProxyService serviceProxy, ILogger<CategoriesController> logger)
    {
        _serviceProxy = serviceProxy;
        _logger = logger;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            _logger.LogInformation("Forwarding GetCategories request to Product Catalog service");

            var path = "api/categories";
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
            _logger.LogError(ex, "Error forwarding GetCategories request");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategory(string categoryId)
    {
        try
        {
            _logger.LogInformation("Forwarding GetCategory request for {CategoryId}", categoryId);

            var path = $"api/categories/{categoryId}";
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
            _logger.LogError(ex, "Error forwarding GetCategory request for {CategoryId}", categoryId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] object categoryData)
    {
        try
        {
            _logger.LogInformation("Forwarding CreateCategory request");

            var path = "api/categories";
            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Post, categoryData);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding CreateCategory request");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategory(string categoryId, [FromBody] object categoryData)
    {
        try
        {
            _logger.LogInformation("Forwarding UpdateCategory request for {CategoryId}", categoryId);

            var path = $"api/categories/{categoryId}";
            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Put, categoryData);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding UpdateCategory request for {CategoryId}", categoryId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(string categoryId)
    {
        try
        {
            _logger.LogInformation("Forwarding DeleteCategory request for {CategoryId}", categoryId);

            var path = $"api/categories/{categoryId}";
            var response = await _serviceProxy.ForwardRequestAsync("ProductCatalog", path, HttpMethod.Delete);
            
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding DeleteCategory request for {CategoryId}", categoryId);
            return StatusCode(500, "Internal server error");
        }
    }
}
