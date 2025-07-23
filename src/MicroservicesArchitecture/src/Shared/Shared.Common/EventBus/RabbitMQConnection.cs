using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace Shared.Common.EventBus;

/// <summary>
/// RabbitMQ persistent connection interface
/// </summary>
public interface IRabbitMQPersistentConnection : IDisposable
{
    bool IsConnected { get; }
    bool TryConnect();
    IModel CreateModel();
}

/// <summary>
/// RabbitMQ persistent connection implementation
/// </summary>
public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
    private readonly int _retryCount;
    private IConnection? _connection;
    private bool _disposed;

    private readonly object sync_root = new object();

    public DefaultRabbitMQPersistentConnection(
        IConnectionFactory connectionFactory,
        ILogger<DefaultRabbitMQPersistentConnection> logger,
        int retryCount = 5)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryCount = retryCount;
    }

    public bool IsConnected
    {
        get
        {
            return _connection != null && _connection.IsOpen && !_disposed;
        }
    }

    public IModel CreateModel()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
        }

        return _connection!.CreateModel();
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        try
        {
            _connection?.Dispose();
        }
        catch (IOException ex)
        {
            _logger.LogCritical(ex, "Error disposing RabbitMQ connection");
        }
    }

    public bool TryConnect()
    {
        _logger.LogInformation("RabbitMQ Client is trying to connect");

        lock (sync_root)
        {
            var policy = RetryPolicy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetryAsync(_retryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s", $"{time.TotalSeconds:n1}");
                }
            );

            policy.ExecuteAsync(() =>
            {
                _connection = _connectionFactory.CreateConnection();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

            if (IsConnected)
            {
                _connection!.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;

                _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                return true;
            }
            else
            {
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;

        _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

        TryConnect();
    }

    void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return;

        _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

        TryConnect();
    }

    void OnConnectionShutdown(object? sender, ShutdownEventArgs reason)
    {
        if (_disposed) return;

        _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

        TryConnect();
    }
}

/// <summary>
/// Simple retry policy implementation
/// </summary>
public static class RetryPolicy
{
    public static RetryPolicyBuilder Handle<TException>() where TException : Exception
    {
        return new RetryPolicyBuilder(typeof(TException));
    }
}

public class RetryPolicyBuilder
{
    private readonly List<Type> _exceptionTypes = new();

    public RetryPolicyBuilder(Type exceptionType)
    {
        _exceptionTypes.Add(exceptionType);
    }

    public RetryPolicyBuilder Or<TException>() where TException : Exception
    {
        _exceptionTypes.Add(typeof(TException));
        return this;
    }

    public RetryPolicyExecutor WaitAndRetryAsync(int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan>? onRetry = null)
    {
        return new RetryPolicyExecutor(_exceptionTypes, retryCount, sleepDurationProvider, onRetry);
    }
}

public class RetryPolicyExecutor
{
    private readonly List<Type> _exceptionTypes;
    private readonly int _retryCount;
    private readonly Func<int, TimeSpan> _sleepDurationProvider;
    private readonly Action<Exception, TimeSpan>? _onRetry;

    public RetryPolicyExecutor(List<Type> exceptionTypes, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan>? onRetry)
    {
        _exceptionTypes = exceptionTypes;
        _retryCount = retryCount;
        _sleepDurationProvider = sleepDurationProvider;
        _onRetry = onRetry;
    }

    public async Task ExecuteAsync(Func<Task> operation)
    {
        var attempts = 0;
        while (true)
        {
            try
            {
                await operation();
                return;
            }
            catch (Exception ex) when (_exceptionTypes.Any(t => t.IsAssignableFrom(ex.GetType())) && attempts < _retryCount)
            {
                attempts++;
                var delay = _sleepDurationProvider(attempts);
                _onRetry?.Invoke(ex, delay);
                await Task.Delay(delay);
            }
        }
    }
}

/// <summary>
/// Extension methods for types
/// </summary>
public static class TypeExtensions
{
    public static string GetGenericTypeName(this Type type)
    {
        var typeName = string.Empty;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    public static string GetGenericTypeName(this object @object)
    {
        return @object.GetType().GetGenericTypeName();
    }
}
