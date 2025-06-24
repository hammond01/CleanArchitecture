using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Net.Http.Json;
using ProductManager.Shared.DTOs.ProductDto;
using Xunit;

namespace ProductManager.IntegrationTests.Controllers;

public class ProductControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    [Fact]
    public async Task GetProducts_ShouldReturnOkResult()
    {
        // Act
        var response = await _client.GetAsync("/v1/Product/Get");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var productDto = new CreateProductDto
        {
            ProductName = "Test Product",
            CategoryId = "test-category",
            SupplierId = "test-supplier",
            UnitPrice = 100.00m,
            UnitsInStock = 50
        };        // Act
        var response = await _client.PostAsJsonAsync("/v1/Product/Post", productDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }
}
