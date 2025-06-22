using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Product.Commands;
using ProductManager.Application.Feature.Product.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.ProductDto;
namespace ProductManager.Api.Controllers;

public class ProductController : ConBase
{
    private readonly Dispatcher _dispatcher;
    private readonly ILogger<ProductController> _logger;

    public ProductController(Dispatcher dispatcher, ILogger<ProductController> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }
    [HttpGet]
    [LogAction("Get all products")]
    public async Task<ApiResponse> GetProducts()
    {
        _logger.LogInformation("🔍 Getting all products");
        var data = await _dispatcher.DispatchAsync(new GetProducts());
        data.Result = ((List<Products>)data.Result).Adapt<List<GetProductDto>>();
        _logger.LogInformation("✅ Retrieved {Count} products", ((List<GetProductDto>)data.Result).Count);
        return data;
    }

    [HttpGet("{id}")]
    [LogAction("Get product by ID")]
    public async Task<ApiResponse> GetProduct(string id)
    {
        _logger.LogInformation("🔍 Getting product with ID: {ProductId}", id);
        var data = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        data.Result = ((Products)data.Result).Adapt<GetProductDto>();
        _logger.LogInformation("✅ Retrieved product: {ProductName}", ((GetProductDto)data.Result).ProductName);
        return data;
    }

    [HttpPost]
    [LogAction("Create new product")]
    public async Task<ApiResponse> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        _logger.LogInformation("➕ Creating new product: {ProductName}", createProductDto.ProductName);
        var data = createProductDto.Adapt<Products>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(data));
        _logger.LogInformation("✅ Product created with status: {StatusCode}", result.StatusCode);
        return result;
    }

    [HttpPut("{id}")]
    [LogAction("Update product")]
    public async Task<ApiResponse> UpdateProduct(string id, [FromBody] UpdateProductDto updateProductDto)
    {
        _logger.LogInformation("✏️ Updating product with ID: {ProductId}", id);
        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        var product = (Products)apiResponse.Result;
        updateProductDto.Adapt(product);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(product));
        _logger.LogInformation("✅ Product updated with status: {StatusCode}", result.StatusCode);
        return result;
    }

    [HttpDelete("{id}")]
    [LogAction("Delete product")]
    public async Task<ApiResponse> DeleteProduct(string id)
    {
        _logger.LogInformation("🗑️ Deleting product with ID: {ProductId}", id);
        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        var product = (Products)apiResponse.Result;
        var result = await _dispatcher.DispatchAsync(new DeleteProductCommand(product));
        _logger.LogInformation("✅ Product deleted with status: {StatusCode}", result.StatusCode);
        return result;
    }
}
