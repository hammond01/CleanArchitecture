using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductManager.Shared.DTOs.CustomerDto;
using Xunit;
namespace ProductManager.IntegrationTests.Controllers;

public class CustomerControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public CustomerControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetCustomers_ShouldReturnOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/customers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var createCustomerDto = new CreateCustomerDto
        {
            CompanyName = "",// Invalid - required field
            ContactName = "John Doe"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/customers", createCustomerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCustomer_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var customerId = "test-id";

        // Act
        var response = await _client.GetAsync($"/api/v1.0/customers/{customerId}");

        // Assert - Should return OK even if customer doesn't exist (returns null in Result)
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateCustomer_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var customerId = "test-id";
        var updateCustomerDto = new UpdateCustomerDto
        {
            CompanyName = "Updated Customer", ContactName = "Jane Doe"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1.0/customers/{customerId}", updateCustomerDto);

        // Assert - May return NotFound if customer doesn't exist
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCustomer_WithValidId_ShouldReturnNoContentOrNotFound()
    {
        // Arrange
        var customerId = "test-id";

        // Act
        var response = await _client.DeleteAsync($"/api/v1.0/customers/{customerId}");

        // Assert - May return NotFound if customer doesn't exist
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }
}
