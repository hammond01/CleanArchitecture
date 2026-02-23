using BuildingBlocks.Api.Controllers;
using BuildingBlocks.Application.Dispatcher;
using Catalog.Application.DTOs;
using Catalog.Application.Features.Products.Commands;
using Catalog.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

/// <summary>
/// Product API Controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : BaseController
{
    private readonly IDispatcher _dispatcher;

    public ProductsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Get all products with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? categoryId = null,
        [FromQuery] bool? discontinued = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetProductsQuery
        {
            CategoryId = categoryId,
            Discontinued = discontinued,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var products = await _dispatcher.DispatchAsync(query);
        return Ok(products);
    }

    /// <summary>
    /// Export all products as a CSV file
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> ExportProducts(CancellationToken cancellationToken = default)
    {
        var csvBytes = await _dispatcher.DispatchAsync(new ExportProductsQuery(), cancellationToken);
        return File(csvBytes, "text/csv", $"products_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(string id)
    {
        var query = new GetProductByIdQuery(id);
        var product = await _dispatcher.DispatchAsync(query);

        if (product == null)
        {
            return NotFound($"Product with ID {id} not found");
        }

        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateOrUpdateProductCommand command)
    {
        var productId = await _dispatcher.DispatchAsync(command);
        return Created($"/api/v1/products/{productId}", new { id = productId });
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] CreateOrUpdateProductCommand command)
    {
        var updateCommand = command with { Id = id };
        var productId = await _dispatcher.DispatchAsync(updateCommand);
        return Ok(new { id = productId });
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var command = new DeleteProductCommand(id);
        var result = await _dispatcher.DispatchAsync(command);

        if (!result)
        {
            return NotFound($"Product with ID {id} not found");
        }

        return Ok(new { message = "Product deleted successfully" });
    }
}
