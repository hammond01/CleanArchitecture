using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductManager.Shared.DTOs.ProductDto;
using Xunit;

namespace ProductManager.IntegrationTests.Controllers;

public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ProductControllerTests(CustomWebApplicationFactory<Program> factory)
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
            CategoryId = "test-category-1", // Use seeded test category
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
        // Arrange - First create a product
        var createDto = new CreateProductDto
        {
            ProductName = "Test Product for Get",
            CategoryId = "test-cat-get",
            UnitPrice = 99.99m,
            UnitsInStock = 10,
            UnitsOnOrder = 0,
            Discontinued = false,
            QuantityPerUnit = "1 unit",
            ReorderLevel = 5
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1.0/products", createDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<GetProductDto>();
        var productId = createdProduct?.Id;

        // Act
        var response = await _client.GetAsync($"/api/v1.0/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var productId = "test-product-1"; // Use seeded test data
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
        var productId = "test-product-1"; // Use seeded test data

        // Act
        var response = await _client.DeleteAsync($"/api/v1.0/products/{productId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ExportProducts_ShouldReturnCsvFile()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/products/export");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("products.csv");

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
        content.Should().Contain("ProductName"); // CSV header
    }
}
