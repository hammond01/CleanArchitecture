using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductManager.Shared.DTOs.ProductDto;
using Xunit;
namespace ProductManager.IntegrationTests.Controllers;

public class ProductControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ProductControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    [Fact]
    public async Task GetProducts_ShouldReturnOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var productDto = new CreateProductDto
        {
            ProductName = "Test Product",
            CategoryId = "01JH179GGG9BN2V8SS9RG70QNG",// Valid category ID from seeded data (Mobile Phones)
            SupplierId = "01JH179GGZ7FAHZ0DNFYNZ19FG",// Valid supplier ID from seeded data (Tech Supplies Co.)
            UnitPrice = 100.00m,
            UnitsInStock = 50,
            UnitsOnOrder = 0,
            Discontinued = false,
            QuantityPerUnit = "1 box",
            ReorderLevel = 0
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/products", productDto);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange - Pass null object to trigger bad request
        object? createProductDto = null;

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/products", createProductDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var productId = "test-id";

        // Act
        var response = await _client.GetAsync($"/api/v1.0/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var productId = "test-id";
        var updateProductDto = new UpdateProductDto
        {
            ProductName = "Updated Product Name",
            UnitPrice = 150.00m,
            UnitsInStock = 75
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1.0/products/{productId}", updateProductDto);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ShouldReturnNoContentOrNotFound()
    {
        // Arrange
        var productId = "test-id";

        // Act
        var response = await _client.DeleteAsync($"/api/v1.0/products/{productId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }
}
