using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.Cache;

/// <summary>
/// Generic cache service interface
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}

/// <summary>
/// Distributed cache service implementation
/// </summary>
public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public DistributedCacheService(
        IDistributedCache cache,
        ILogger<DistributedCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedValue = await _cache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrWhiteSpace(cachedValue))
                return default;

            return JsonSerializer.Deserialize<T>(cachedValue, _serializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving value from cache with key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, _serializerOptions);

            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Default 30 minutes
            }

            await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
            _logger.LogDebug("Cache entry set for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Cache entry removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Note: This is a simplified implementation
        // For Redis, you would use Redis-specific commands
        _logger.LogWarning("RemoveByPatternAsync is not fully implemented for generic IDistributedCache");
        await Task.CompletedTask;
    }
}

/// <summary>
/// Cache decorator pattern can be implemented at the service level
/// This provides a simple cache service for business logic
/// </summary>
public class BusinessCacheService
{
    private readonly ICacheService _cache;
    private readonly ILogger<BusinessCacheService> _logger;

    public BusinessCacheService(ICacheService cache, ILogger<BusinessCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> getItem,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var cachedValue = await _cache.GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return cachedValue;
        }

        _logger.LogDebug("Cache miss for key: {Key}", key);
        var item = await getItem();
        if (item != null)
        {
            await _cache.SetAsync(key, item, expiration, cancellationToken);
        }

        return item;
    }
}

/// <summary>
/// Cache key generator
/// </summary>
public static class CacheKeys
{
    public static string Product(string id) => $"product:{id}";
    public static string Products => "products:all";
    public static string ProductsByCategory(string categoryId) => $"products:category:{categoryId}";
    public static string Category(string id) => $"category:{id}";
    public static string Categories => "categories:all";
    public static string User(string id) => $"user:{id}";
}
