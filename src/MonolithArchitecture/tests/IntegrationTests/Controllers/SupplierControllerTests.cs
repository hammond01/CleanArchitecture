using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductManager.Shared.DTOs.SupplierDto;
using Xunit;
namespace ProductManager.IntegrationTests.Controllers;

public class SupplierControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public SupplierControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetSuppliers_ShouldReturnOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/suppliers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateSupplier_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange - Pass null object to trigger bad request
        object? createSupplierDto = null;

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/suppliers", createSupplierDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSupplier_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var supplierId = "test-id";

        // Act
        var response = await _client.GetAsync($"/api/v1.0/suppliers/{supplierId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateSupplier_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var supplierId = "test-id";
        var updateSupplierDto = new UpdateSupplierDto
        {
            CompanyName = "Updated Supplier Co",
            ContactName = "Jane Supplier"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1.0/suppliers/{supplierId}", updateSupplierDto);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSupplier_WithValidId_ShouldReturnNoContentOrNotFound()
    {
        // Arrange
        var supplierId = "test-id";

        // Act
        var response = await _client.DeleteAsync($"/api/v1.0/suppliers/{supplierId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }
}
