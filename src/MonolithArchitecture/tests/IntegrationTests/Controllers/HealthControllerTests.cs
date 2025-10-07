using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ProductManager.IntegrationTests.Controllers;

public class HealthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public HealthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ShouldReturnOkWithHealthStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("status", out var statusProperty).Should().BeTrue();
        statusProperty.GetString().Should().Be("Healthy");

        jsonDocument.RootElement.TryGetProperty("timestamp", out var timestampProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("version", out var versionProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("environment", out var environmentProperty).Should().BeTrue();
    }

    [Fact]
    public async Task GetDetailedHealth_ShouldReturnOkWithDetailedInfo()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/health/detailed");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("status", out var statusProperty).Should().BeTrue();
        statusProperty.GetString().Should().Be("Healthy");

        jsonDocument.RootElement.TryGetProperty("database", out var databaseProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("system", out var systemProperty).Should().BeTrue();
    }

    [Fact]
    public async Task GetDatabaseHealth_ShouldReturnOkWithDatabaseInfo()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/health/database");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("status", out var statusProperty).Should().BeTrue();
        statusProperty.GetString().Should().Be("Healthy");
    }

    [Fact]
    public async Task GetSystemInfo_ShouldReturnOkWithSystemInfo()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/health/system");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("machineName", out var machineNameProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("osVersion", out var osVersionProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("processorCount", out var processorCountProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("frameworkVersion", out var frameworkVersionProperty).Should().BeTrue();
    }
}
