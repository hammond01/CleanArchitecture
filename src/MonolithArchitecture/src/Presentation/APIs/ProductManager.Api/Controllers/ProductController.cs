using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Product.Commands;
using ProductManager.Application.Feature.Product.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Infrastructure.Middleware;
using ProductManager.Shared.DTOs.ProductDto;
namespace ProductManager.Api.Controllers;

[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public ProductController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [LogAction("Get all products")]
    public async Task<ActionResult<ApiResponse>> GetProducts()
    {
        var data = await _dispatcher.DispatchAsync(new GetProducts());
        data.Result = data.Result.Adapt<List<GetProductDto>>();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [LogAction("Get product by ID")]
    public async Task<ActionResult<ApiResponse>> GetProduct(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        data.Result = data.Result.Adapt<GetProductDto>();
        return Ok(data);
    }

    [HttpPost]
    [LogAction("Create new product")]
    public async Task<ActionResult<ApiResponse>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var data = createProductDto.Adapt<Products>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(data));
        var createdProduct = (Products)result.Result;
        return Created($"/api/v1.0/products/{createdProduct.Id}", result);
    }

    [HttpPut("{id}")]
    [EntityLock("Product", "{id}", 30)]
    [LogAction("Update product")]
    public async Task<ActionResult<ApiResponse>> UpdateProduct(string id, [FromBody] UpdateProductDto updateProductDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        if (apiResponse.Result is not Products product)
        {
            return NotFound(new ApiResponse(404, "Product not found"));
        }
        updateProductDto.Adapt(product);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(product));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [LogAction("Delete product")]
    public async Task<ActionResult<ApiResponse>> DeleteProduct(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        if (apiResponse.Result is not Products product)
        {
            return NotFound(new ApiResponse(404, "Product not found"));
        }
        await _dispatcher.DispatchAsync(new DeleteProductCommand(product));
        return NoContent();
    }
}
