using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ProductManager.IntegrationTests.Controllers;

public class LockTestControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public LockTestControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task TestExplicitLock_ShouldReturnOk()
    {
        // Arrange
        var resourceId = "test-resource-123";
        var testData = "Test lock data";
        var content = new StringContent(JsonSerializer.Serialize(testData), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/v1.0/lock-tests/explicit/{resourceId}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task TestManualLock_ShouldReturnOk()
    {
        // Arrange
        var resourceId = "manual-resource-456";
        var testData = "Manual lock test data";
        var content = new StringContent(JsonSerializer.Serialize(testData), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/v1.0/lock-tests/manual/{resourceId}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task TestLockStatus_ShouldReturnOk()
    {
        // Arrange
        var resourceId = "status-resource-202";

        // Act
        var response = await _client.GetAsync($"/api/v1.0/lock-tests/status/{resourceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }
}
