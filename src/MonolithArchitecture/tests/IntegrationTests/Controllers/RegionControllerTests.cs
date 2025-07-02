using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductManager.Application.Feature.Region.DTOs;
using ProductManager.Domain.Common;
using Xunit;

namespace ProductManager.IntegrationTests.Controllers;

public class RegionControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public RegionControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetRegions_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync("/api/v1.0/regions");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateUpdateDeleteRegion_WorksCorrectly()
    {
        // Create
        var create = new CreateRegionRequest { RegionDescription = "Test Region", TestCode = "TEST001" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1.0/regions", create);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse>();
        var id = ((System.Text.Json.JsonElement)created!.Result).GetProperty("id").GetString();

        // Update
        var update = new UpdateRegionRequest { RegionDescription = "Updated Region", TestCode = "TEST002" };
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1.0/regions/{id}", update);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/api/v1.0/regions/{id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
