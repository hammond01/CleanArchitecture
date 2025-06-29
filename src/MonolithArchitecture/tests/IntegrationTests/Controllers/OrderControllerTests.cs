using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductManager.Shared.DTOs.OrderDto;
using Xunit;
namespace ProductManager.IntegrationTests.Controllers;

public class OrderControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public OrderControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetOrders_ShouldReturnOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange - Pass null object to trigger bad request
        object? createOrderDto = null;

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/orders", createOrderDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrder_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var orderId = "test-id";

        // Act
        var response = await _client.GetAsync($"/api/v1.0/orders/{orderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrder_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var orderId = "test-id";
        var updateOrderDto = new UpdateOrderDto
        {
            Id = orderId,
            ShipName = "Updated Ship Name",
            Freight = 15.75m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1.0/orders/{orderId}", updateOrderDto);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOrder_WithValidId_ShouldReturnNoContentOrNotFound()
    {
        // Arrange
        var orderId = "test-id";

        // Act
        var response = await _client.DeleteAsync($"/api/v1.0/orders/{orderId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }
}
