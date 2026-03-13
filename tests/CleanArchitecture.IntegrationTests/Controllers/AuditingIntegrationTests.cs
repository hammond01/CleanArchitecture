using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using CleanArchitecture.IntegrationTests.Infrastructure;

namespace CleanArchitecture.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for Auditing API endpoints
/// </summary>
public class AuditingIntegrationTests : IClassFixture<SqlServerWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuditingIntegrationTests(SqlServerWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAuditEntries_ReturnsMutationAuditEntries()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        await _client.PostAsJsonAsync("/api/v1/categories", new
        {
            CategoryName = "Audit Source Category"
        });

        // Act
        var response = await _client.GetAsync("/api/v1/auditlogs");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"success\"");
        content.Should().Contain("\"data\"");
        content.Should().Contain("POST /api/v1/categories");
    }
}
