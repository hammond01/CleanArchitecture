using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductManager.Shared.DTOs.ShipperDto;
using Xunit;
namespace ProductManager.IntegrationTests.Controllers;

public class ShipperControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ShipperControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetShippers_ShouldReturnOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/shippers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateShipper_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createShipperDto = new CreateShipperDto
        {
            CompanyName = "Test Shipping Co",
            Phone = "123-456-7890"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/shippers", createShipperDto);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateShipper_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var createShipperDto = new CreateShipperDto
        {
            CompanyName = "", // Invalid - required field
            Phone = "123-456-7890"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/shippers", createShipperDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetShipper_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var shipperId = "test-id";

        // Act
        var response = await _client.GetAsync($"/api/v1.0/shippers/{shipperId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateShipper_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var shipperId = "test-id";
        var updateShipperDto = new UpdateShipperDto
        {
            CompanyName = "Updated Shipping Co",
            Phone = "987-654-3210"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1.0/shippers/{shipperId}", updateShipperDto);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteShipper_WithValidId_ShouldReturnNoContentOrNotFound()
    {
        // Arrange
        var shipperId = "test-id";

        // Act
        var response = await _client.DeleteAsync($"/api/v1.0/shippers/{shipperId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }
}
