using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Product.Commands;
using ProductManager.Application.Feature.Product.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Infrastructure.Middleware;
using ProductManager.Shared.DTOs.ProductDto;
using System.ComponentModel.DataAnnotations;

namespace ProductManager.Api.Controllers;

/// <summary>
/// Product management endpoints
/// </summary>
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
[ApiController]
// [Authorize]
[Produces("application/json")]
[ProducesResponseType(401, Type = typeof(ApiResponse))]
[ProducesResponseType(500, Type = typeof(ApiResponse))]
public class ProductController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public ProductController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>List of products</returns>
    [HttpGet]
    [LogAction("Get all products")]
    [AllowAnonymous]
    [ProducesResponseType(200, Type = typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetProducts()
    {
        var data = await _dispatcher.DispatchAsync(new GetProducts());
        data.Result = data.Result.Adapt<List<GetProductDto>>();

        // Add response headers for better API experience
        if (data.Result is ICollection<GetProductDto> products)
        {
            Response.Headers["X-Total-Count"] = products.Count.ToString();
        }

        return Ok(data);
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    [LogAction("Get product by ID")]
    [AllowAnonymous]
    [ProducesResponseType(200, Type = typeof(ApiResponse))]
    [ProducesResponseType(404, Type = typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetProduct([Required] string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        if (data.Result == null)
        {
            return NotFound(new ApiResponse(404, "Product not found"));
        }

        data.Result = data.Result.Adapt<GetProductDto>();
        return Ok(data);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="createProductDto">Product creation data</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [LogAction("Create new product")]
    [ProducesResponseType(201, Type = typeof(ApiResponse))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse(400, "Invalid data", ModelState));
        }

        var data = createProductDto.Adapt<Products>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(data));
        var createdProduct = (Products)result.Result;

        return Created($"/api/v1.0/products/{createdProduct.Id}", result);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="updateProductDto">Product update data</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    [EntityLock("Product", "{id}", 30)]
    [LogAction("Update product")]
    [ProducesResponseType(200, Type = typeof(ApiResponse))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    [ProducesResponseType(404, Type = typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> UpdateProduct([Required] string id, [FromBody] UpdateProductDto updateProductDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse(400, "Invalid data", ModelState));
        }

        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        if (apiResponse.Result is not Products product)
        {
            return NotFound(new ApiResponse(404, "Product not found"));
        }

        updateProductDto.Adapt(product);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(product));
        return Ok(result);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [LogAction("Delete product")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404, Type = typeof(ApiResponse))]
    public async Task<ActionResult> DeleteProduct([Required] string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        if (apiResponse.Result is not Products product)
        {
            return NotFound(new ApiResponse(404, "Product not found"));
        }

        await _dispatcher.DispatchAsync(new DeleteProductCommand(product));
        return NoContent();
    }

    /// <summary>
    /// Export products to CSV
    /// </summary>
    /// <returns>CSV file</returns>
    [HttpGet("export")]
    [LogAction("Export products to CSV")]
    [AllowAnonymous]
    [Produces("text/csv")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ExportProducts()
    {
        var csvBytes = await _dispatcher.DispatchAsync(new ExportProducts());
        return File(csvBytes, "text/csv", "products.csv");
    }
}
