using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Persistence;
using Xunit;

namespace ProductManager.IntegrationTests.Controllers;

public class LogsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public LogsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetApiLogs_ShouldReturnOkWithPaginatedData()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/logs/api");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("data", out var dataProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("totalCount", out var totalCountProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("page", out var pageProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("pageSize", out var pageSizeProperty).Should().BeTrue();
    }

    [Fact]
    public async Task GetApiLogs_WithPagination_ShouldReturnCorrectPage()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/logs/api?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);

        jsonDocument.RootElement.GetProperty("page").GetInt32().Should().Be(1);
        jsonDocument.RootElement.GetProperty("pageSize").GetInt32().Should().Be(10);
    }

    [Fact]
    public async Task GetApiLogs_WithDateFilter_ShouldReturnOk()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd");
        var toDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

        // Act
        var response = await _client.GetAsync($"/api/v1.0/logs/api?fromDate={fromDate}&toDate={toDate}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetActionLogs_ShouldReturnOkWithPaginatedData()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/logs/actions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("data", out var dataProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("totalCount", out var totalCountProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("page", out var pageProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("pageSize", out var pageSizeProperty).Should().BeTrue();
    }

    [Fact]
    public async Task GetActionLogs_WithActionNameFilter_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/logs/actions?actionName=TestAction");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLogStatistics_ShouldReturnOkWithStatisticsData()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/logs/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("apiLogs", out var apiLogsProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("actionLogs", out var actionLogsProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("topEndpoints", out var topEndpointsProperty).Should().BeTrue();
    }
}
