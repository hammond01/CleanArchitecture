using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductManager.Shared.DTOs.CategoryDto;
using Xunit;

namespace ProductManager.IntegrationTests.Controllers;

public class CategoryControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CategoryControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ShouldReturnOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateCategory_WithValidData_ShouldNotReturnServerError()
    {
        // Arrange
        var createCategoryDto = new CreateCategoryDto
        {
            CategoryName = "Test Category",
            Description = "Test description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/categories", createCategoryDto);

        // Assert - Controller should handle request without 500 error
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateCategory_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange - Pass null object to trigger bad request
        object? createCategoryDto = null;

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/categories", createCategoryDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCategory_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var categoryId = "test-category-1"; // Use seeded test data

        // Act
        var response = await _client.GetAsync($"/api/v1.0/categories/{categoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateCategory_WithValidData_ShouldNotReturnServerError()
    {
        // Arrange
        var categoryId = "test-category-1"; // Use seeded test data
        var updateCategoryDto = new UpdateCategoryDto
        {
            CategoryName = "Electronics",
            Description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1.0/categories/{categoryId}", updateCategoryDto);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task DeleteCategory_WithValidId_ShouldNotReturnServerError()
    {
        // Arrange
        var categoryId = "test-category-1"; // Use seeded test data

        // Act
        var response = await _client.DeleteAsync($"/api/v1.0/categories/{categoryId}");

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }
}
