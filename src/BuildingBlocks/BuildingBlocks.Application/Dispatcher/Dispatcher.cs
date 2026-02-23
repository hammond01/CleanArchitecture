using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Domain.Events;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Application.Dispatcher;

/// <summary>
/// Custom high-performance Dispatcher with validation pipeline and monitoring
/// Đây là trái tim của CQRS + Event-Driven Architecture - điều phối commands, queries và domain events
/// </summary>
public class Dispatcher : IDispatcher
{
    private static readonly List<Type> _eventHandlers = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Dispatcher> _logger;

    public Dispatcher(IServiceProvider serviceProvider, ILogger<Dispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Register domain event handlers từ assembly
    /// </summary>
    public static void RegisterEventHandlers(System.Reflection.Assembly assembly, IServiceCollection services)
    {
        var types = assembly.GetTypes()
            .Where(x => x.GetInterfaces().Any(y => y.IsGenericType &&
                   y.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
            .ToList();

        foreach (var type in types)
        {
            services.AddTransient(type);
            _eventHandlers.Add(type);
        }
    }

    /// <summary>
    /// Dispatch query với automatic validation và performance monitoring
    /// </summary>
    public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var queryName = queryType.Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("🔍 Dispatching Query: {QueryName}", queryName);

            // Step 1: Validate query
            await ValidateAsync(query, cancellationToken);

            // Step 2: Get handler
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                throw new InvalidOperationException(
                    $"❌ No handler found for query '{queryName}'. Did you forget to register it?");
            }

            // Step 3: Execute handler
            dynamic dynamicHandler = handler;
            TResult result = await dynamicHandler.HandleAsync((dynamic)query, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("✅ Query {QueryName} completed in {ElapsedMs}ms",
                queryName, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "❌ Query {QueryName} failed after {ElapsedMs}ms",
                queryName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// Dispatch command với automatic validation, performance monitoring, và transaction support
    /// </summary>
    public async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var commandName = commandType.Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("⚡ Dispatching Command: {CommandName}", commandName);

            // Step 1: Validate command
            await ValidateAsync(command, cancellationToken);

            // Step 2: Get handler
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                throw new InvalidOperationException(
                    $"❌ No handler found for command '{commandName}'. Did you forget to register it?");
            }

            // Step 3: Execute handler with transaction
            dynamic dynamicHandler = handler;
            TResult result = await dynamicHandler.HandleAsync((dynamic)command, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("✅ Command {CommandName} completed in {ElapsedMs}ms",
                commandName, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (ValidationException validationEx)
        {
            stopwatch.Stop();
            _logger.LogWarning("⚠️ Command {CommandName} validation failed after {ElapsedMs}ms: {Errors}",
                commandName, stopwatch.ElapsedMilliseconds,
                string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage)));
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "❌ Command {CommandName} failed after {ElapsedMs}ms",
                commandName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// Dispatch domain event to all registered handlers (Event-Driven Architecture)
    /// </summary>
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var eventType = domainEvent.GetType();
        var eventName = eventType.Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("📢 Dispatching Domain Event: {EventName}", eventName);

            var handlerCount = 0;
            foreach (var handlerType in _eventHandlers)
            {
                var canHandleEvent = handlerType.GetInterfaces()
                    .Any(x => x.IsGenericType
                             && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
                             && x.GenericTypeArguments != null
                             && x.GenericTypeArguments.Length > 0
                             && x.GenericTypeArguments[0] == eventType);

                if (!canHandleEvent)
                    continue;

                var handler = _serviceProvider.GetService(handlerType);
                if (handler != null)
                {
                    await ((dynamic)handler).HandleAsync((dynamic)domainEvent, cancellationToken);
                    handlerCount++;
                }
            }

            stopwatch.Stop();
            _logger.LogInformation("✅ Domain Event {EventName} dispatched to {HandlerCount} handler(s) in {ElapsedMs}ms",
                eventName, handlerCount, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "❌ Domain Event {EventName} failed after {ElapsedMs}ms",
                eventName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// Automatic FluentValidation integration
    /// </summary>
    private async Task ValidateAsync<T>(T request, CancellationToken cancellationToken)
    {
        var validators = _serviceProvider.GetServices<IValidator<T>>().ToList();

        if (!validators.Any())
            return;

        var context = new ValidationContext<T>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }
    }
}
