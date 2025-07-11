using Shared.Common.Mediator;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Products.Commands;
using ProductCatalog.Application.Products.Queries;
using ProductCatalog.Domain.Entities;

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
    public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get products with pagination
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsPagedQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(string id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound($"Product with ID {id} not found");

        return Ok(result);
    }

    /// <summary>
    /// Create new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<string>> CreateProduct([FromBody] CreateProductCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProductById), new { id = result }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(string id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.ProductId)
            return BadRequest("Product ID mismatch");

        try
        {
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete product
    /// </summary>
    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(string id)
    {
        // Implementation for delete would go here
        // For now, return NotImplemented
        return StatusCode(501, "Delete operation not implemented yet");
    }
}
