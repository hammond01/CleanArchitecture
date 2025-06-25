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

public class ProductController : ConBase
{
    private readonly Dispatcher _dispatcher;

    public ProductController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [LogAction("Get all products")]
    public async Task<ApiResponse> Get()
    {
        return await _dispatcher.DispatchAsync(new GetProducts());
    }

    [HttpGet("{id}")]
    [LogAction("Get product by ID")]
    public async Task<ApiResponse> Get(string id)
    {
        return await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
    }

    [HttpPost]
    [LogAction("Create new product")]
    public async Task<ApiResponse> Post([FromBody] CreateProductDto createProductDto)
    {
        var data = createProductDto.Adapt<Products>();
        return await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(data));
    }

    [HttpPut("{id}")]
    [EntityLock("Product", "{id}", 30)]
    [LogAction("Update product")]
    public async Task<ApiResponse> Put(string id, [FromBody] UpdateProductDto updateProductDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        if (apiResponse?.Result is Products product)
        {
            updateProductDto.Adapt(product);
            return await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(product));
        }
        return new ApiResponse(404, "Product not found");
    }

    [HttpDelete("{id}")]
    [LogAction("Delete product")]
    public async Task<ApiResponse> Delete(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        if (apiResponse?.Result is Products product)
        {
            return await _dispatcher.DispatchAsync(new DeleteProductCommand(product));
        }
        return new ApiResponse(404, "Product not found");
    }
}
