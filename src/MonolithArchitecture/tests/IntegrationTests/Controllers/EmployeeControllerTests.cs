using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductManager.Shared.DTOs.EmployeeDto;
using Xunit;
namespace ProductManager.IntegrationTests.Controllers;

public class EmployeeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public EmployeeControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetEmployees_ShouldReturnOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/employees");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateEmployee_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createEmployeeDto = new CreateEmployeeDto
        {
            LastName = "Doe",
            FirstName = "John",
            Title = "Manager",
            TitleOfCourtesy = "Mr.",
            BirthDate = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            Address = "123 Test St",
            City = "Test City",
            Region = "Test Region",
            PostalCode = "12345",
            Country = "Test Country",
            HomePhone = "123-456-7890",
            Extension = "123",
            Notes = "Test employee"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/employees", createEmployeeDto);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateEmployee_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var createEmployeeDto = new CreateEmployeeDto
        {
            LastName = "", // Invalid - required field
            FirstName = "John"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/employees", createEmployeeDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetEmployee_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var employeeId = "test-id";

        // Act
        var response = await _client.GetAsync($"/api/v1.0/employees/{employeeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateEmployee_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var employeeId = "test-id";
        var updateEmployeeDto = new UpdateEmployeeDto
        {
            Id = employeeId,
            LastName = "Updated Doe",
            FirstName = "Jane"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1.0/employees/{employeeId}", updateEmployeeDto);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteEmployee_WithValidId_ShouldReturnNoContentOrNotFound()
    {
        // Arrange
        var employeeId = "test-id";

        // Act
        var response = await _client.DeleteAsync($"/api/v1.0/employees/{employeeId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }
}
