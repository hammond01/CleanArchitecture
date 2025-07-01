using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ProductManager.IntegrationTests.Controllers;

public class SearchControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SearchControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GlobalSearch_WithValidQuery_ShouldReturnResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/search/global?query=test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("query", out var queryProperty).Should().BeTrue();
        queryProperty.GetString().Should().Be("test");

        jsonDocument.RootElement.TryGetProperty("totalResults", out var totalResultsProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("results", out var resultsProperty).Should().BeTrue();

        resultsProperty.TryGetProperty("products", out var productsProperty).Should().BeTrue();
        resultsProperty.TryGetProperty("customers", out var customersProperty).Should().BeTrue();
        resultsProperty.TryGetProperty("employees", out var employeesProperty).Should().BeTrue();
        resultsProperty.TryGetProperty("categories", out var categoriesProperty).Should().BeTrue();
    }

    [Fact]
    public async Task GlobalSearch_WithShortQuery_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/search/global?query=a");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GlobalSearch_WithEmptyQuery_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/search/global?query=");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdvancedProductSearch_WithNoFilters_ShouldReturnPaginatedResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/search/products/advanced");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("data", out var dataProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("totalCount", out var totalCountProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("page", out var pageProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("pageSize", out var pageSizeProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("totalPages", out var totalPagesProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("filters", out var filtersProperty).Should().BeTrue();
    }

    [Fact]
    public async Task AdvancedProductSearch_WithNameFilter_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/search/products/advanced?name=product");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("filters", out var filtersProperty).Should().BeTrue();
        filtersProperty.TryGetProperty("name", out var nameProperty).Should().BeTrue();
        nameProperty.GetString().Should().Be("product");
    }

    [Fact]
    public async Task AdvancedProductSearch_WithPriceRange_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/search/products/advanced?minPrice=10&maxPrice=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);

        jsonDocument.RootElement.TryGetProperty("filters", out var filtersProperty).Should().BeTrue();
        filtersProperty.TryGetProperty("minPrice", out var minPriceProperty).Should().BeTrue();
        filtersProperty.TryGetProperty("maxPrice", out var maxPriceProperty).Should().BeTrue();

        minPriceProperty.GetDecimal().Should().Be(10);
        maxPriceProperty.GetDecimal().Should().Be(100);
    }

    [Fact]
    public async Task SearchCustomers_WithNoFilters_ShouldReturnPaginatedResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/search/customers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        jsonDocument.RootElement.TryGetProperty("data", out var dataProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("totalCount", out var totalCountProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("page", out var pageProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("pageSize", out var pageSizeProperty).Should().BeTrue();
        jsonDocument.RootElement.TryGetProperty("filters", out var filtersProperty).Should().BeTrue();
    }

    [Fact]
    public async Task SearchCustomers_WithQueryFilter_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/search/customers?query=company");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);

        jsonDocument.RootElement.TryGetProperty("filters", out var filtersProperty).Should().BeTrue();
        filtersProperty.TryGetProperty("query", out var queryProperty).Should().BeTrue();
        queryProperty.GetString().Should().Be("company");
    }

    [Fact]
    public async Task SearchCustomers_WithLocationFilters_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/search/customers?city=London&country=UK");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);

        jsonDocument.RootElement.TryGetProperty("filters", out var filtersProperty).Should().BeTrue();
        filtersProperty.TryGetProperty("city", out var cityProperty).Should().BeTrue();
        filtersProperty.TryGetProperty("country", out var countryProperty).Should().BeTrue();

        cityProperty.GetString().Should().Be("London");
        countryProperty.GetString().Should().Be("UK");
    }
}
