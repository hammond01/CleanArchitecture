using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Products.Commands;
using ProductCatalog.Application.Products.Queries;
using Shared.Common.Mediator;

namespace ProductCatalog.API.Controllers;

/// <summary>
/// Products controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var query = new GetAllProductsQuery();
        var products = await _mediator.Send(query);
        return Ok(products);
    }
    /// <summary>
    /// Get low stock products
    /// </summary>
    [HttpGet("low-stock")]
    public ActionResult<IEnumerable<object>> GetLowStockProducts()
    {
        // Mock implementation for now
        var lowStockProducts = new[]
        {
        new { ProductId = "01JH179GGZ7FAHZ0DNFYNZ20AA", ProductName = "Laptop Dell XPS 13", UnitsInStock = 5 },
        new { ProductId = "01JH179GGZ7FAHZ0DNFYNZ22CC", ProductName = "iPhone 15 Pro", UnitsInStock = 3 }
    };

        return Ok(lowStockProducts);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(string id)
    {
        var query = new GetProductByIdQuery { ProductId = id };
        var product = await _mediator.Send(query);

        if (product == null)
            return NotFound($"Product with ID {id} not found");

        return Ok(product);
    }

    /// <summary>
    /// Create new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<string>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { id = productId }, productId);
    }

    /// <summary>
    /// Update product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(string id, [FromBody] UpdateProductCommand command)
    {
        command.ProductId = id;
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound($"Product with ID {id} not found");

        return NoContent();
    }

    /// <summary>
    /// Delete product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(string id)
    {
        var command = new DeleteProductCommand { ProductId = id };
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound($"Product with ID {id} not found");

        return NoContent();
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string categoryId)
    {
        var query = new GetProductsByCategoryQuery { CategoryId = categoryId };
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    /// <summary>
    /// Get products by supplier
    /// </summary>
    [HttpGet("supplier/{supplierId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsBySupplier(string supplierId)
    {
        var query = new GetProductsBySupplierQuery { SupplierId = supplierId };
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public ActionResult<object> HealthCheck()
    {
        return Ok(new { Status = "Healthy", Service = "ProductCatalog API", Timestamp = DateTime.UtcNow });
    }
}

