using System.Text;
using System.Text.Json;
using Polly;
using Polly.Extensions.Http;

namespace ApiGateway.Services;

/// <summary>
/// Service for proxying requests to microservices
/// </summary>
public interface IServiceProxyService
{
    Task<HttpResponseMessage> ForwardRequestAsync(string serviceName, string path, HttpMethod method, object? body = null, Dictionary<string, string>? headers = null);
}

/// <summary>
/// Implementation of service proxy service
/// </summary>
public class ServiceProxyService : IServiceProxyService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ServiceProxyService> _logger;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    public ServiceProxyService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ServiceProxyService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;

        // Configure retry policy with exponential backoff
        _retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (_, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Retry {RetryCount} for {ServiceName} after {Delay}ms",
                        retryCount, context.OperationKey, timespan.TotalMilliseconds);
                });
    }

    public async Task<HttpResponseMessage> ForwardRequestAsync(
        string serviceName,
        string path,
        HttpMethod method,
        object? body = null,
        Dictionary<string, string>? headers = null)
    {
        var baseUrl = _configuration[$"Services:{serviceName}:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
        {
            _logger.LogError("Base URL not configured for service {ServiceName}", serviceName);
            throw new InvalidOperationException($"Base URL not configured for service {serviceName}");
        }

        var httpClient = _httpClientFactory.CreateClient();
        var requestUri = $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";

        using var request = new HttpRequestMessage(method, requestUri);

        // Add headers
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        // Add body for POST/PUT requests
        if (body != null && (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch))
        {
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        try
        {
            _logger.LogInformation("Forwarding {Method} request to {ServiceName}: {RequestUri}",
                method, serviceName, requestUri);

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                var httpResponse = await httpClient.SendAsync(request);
                return httpResponse;
            });

            _logger.LogInformation("Received response from {ServiceName}: {StatusCode}",
                serviceName, response.StatusCode);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding request to {ServiceName}: {RequestUri}",
                serviceName, requestUri);
            throw;
        }
    }
}
