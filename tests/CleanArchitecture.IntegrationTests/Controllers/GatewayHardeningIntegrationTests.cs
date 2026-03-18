using System.Net;
using System.Net.Http.Json;
using CleanArchitecture.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace CleanArchitecture.IntegrationTests.Controllers;

public class GatewayHardeningIntegrationTests : IClassFixture<PostgresWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GatewayHardeningIntegrationTests(PostgresWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoints_ReturnHealthyResponses()
    {
        var liveResponse = await _client.GetAsync("/health/live");
        var readyResponse = await _client.GetAsync("/health/ready");

        liveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        readyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var liveContent = await liveResponse.Content.ReadAsStringAsync();
        var readyContent = await readyResponse.Content.ReadAsStringAsync();

        liveContent.Should().Contain("\"status\":\"Healthy\"");
        readyContent.Should().Contain("\"status\":\"Healthy\"");
    }

    [Fact]
    public async Task SecurityHeaders_ArePresentOnResponses()
    {
        var response = await _client.GetAsync("/health/live");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.TryGetValues("X-Content-Type-Options", out var contentTypeOptions).Should().BeTrue();
        response.Headers.TryGetValues("X-Frame-Options", out var frameOptions).Should().BeTrue();
        response.Headers.TryGetValues("Referrer-Policy", out var referrerPolicy).Should().BeTrue();
        response.Headers.TryGetValues("Content-Security-Policy", out var csp).Should().BeTrue();

        contentTypeOptions!.Single().Should().Be("nosniff");
        frameOptions!.Single().Should().Be("DENY");
        referrerPolicy!.Single().Should().Be("no-referrer");
        csp!.Single().Should().Contain("default-src 'none'");
    }

    [Fact]
    public async Task LoginEndpoint_WhenFlooded_ReturnsTooManyRequests()
    {
        HttpResponseMessage? lastResponse = null;

        for (var attempt = 0; attempt < 35; attempt++)
        {
            lastResponse = await _client.PostAsJsonAsync("/api/v1/authentication/login", new
            {
                UserName = "unknown-user",
                Password = "wrong-password",
                RememberMe = false
            });
        }

        lastResponse.Should().NotBeNull();
        lastResponse!.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}
