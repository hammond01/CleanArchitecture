using BuildingBlocks.Api.Controllers;
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
    private readonly GetProductsQueryHandler _getProductsHandler;
    private readonly GetProductByIdQueryHandler _getProductByIdHandler;
    private readonly CreateOrUpdateProductCommandHandler _createOrUpdateHandler;
    private readonly DeleteProductCommandHandler _deleteHandler;

    public ProductsController(
        GetProductsQueryHandler getProductsHandler,
        GetProductByIdQueryHandler getProductByIdHandler,
        CreateOrUpdateProductCommandHandler createOrUpdateHandler,
        DeleteProductCommandHandler deleteHandler)
    {
        _getProductsHandler = getProductsHandler;
        _getProductByIdHandler = getProductByIdHandler;
        _createOrUpdateHandler = createOrUpdateHandler;
        _deleteHandler = deleteHandler;
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

        var products = await _getProductsHandler.HandleAsync(query);
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(string id)
    {
        var query = new GetProductByIdQuery(id);
        var product = await _getProductByIdHandler.HandleAsync(query);

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
        var productId = await _createOrUpdateHandler.HandleAsync(command);
        return Created($"/api/v1/products/{productId}", new { id = productId });
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] CreateOrUpdateProductCommand command)
    {
        var updateCommand = command with { Id = id };
        var productId = await _createOrUpdateHandler.HandleAsync(updateCommand);
        return Ok(new { id = productId });
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var command = new DeleteProductCommand(id);
        var result = await _deleteHandler.HandleAsync(command);

        if (!result)
        {
            return NotFound($"Product with ID {id} not found");
        }

        return Ok(new { message = "Product deleted successfully" });
    }
}
