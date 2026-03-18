using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Domain.Events;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace BuildingBlocks.Application.Dispatcher;

/// <summary>
/// Custom high-performance Dispatcher with validation pipeline and monitoring
/// Đây là trái tim của CQRS + Event-Driven Architecture - điều phối commands, queries và domain events
/// </summary>
public class Dispatcher : IDispatcher
{
    private static readonly MethodInfo InvokeQueryHandlerMethod = typeof(Dispatcher)
        .GetMethod(nameof(InvokeQueryHandlerAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo InvokeCommandHandlerMethod = typeof(Dispatcher)
        .GetMethod(nameof(InvokeCommandHandlerAsync), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo InvokeDomainEventHandlerMethod = typeof(Dispatcher)
        .GetMethod(nameof(InvokeDomainEventHandlerAsync), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly ConcurrentDictionary<(Type RequestType, Type ResultType), Func<object, object, CancellationToken, Task<object?>>> QueryInvokers = new();
    private static readonly ConcurrentDictionary<(Type RequestType, Type ResultType), Func<object, object, CancellationToken, Task<object?>>> CommandInvokers = new();
    private static readonly ConcurrentDictionary<Type, Func<object, IDomainEvent, CancellationToken, Task>> DomainEventInvokers = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Dispatcher> _logger;

    public Dispatcher(IServiceProvider serviceProvider, ILogger<Dispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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
            var invoker = QueryInvokers.GetOrAdd(
                (queryType, typeof(TResult)),
                static key => BuildQueryInvoker(key.RequestType, key.ResultType));

            var result = await invoker(handler, query, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("✅ Query {QueryName} completed in {ElapsedMs}ms",
                queryName, stopwatch.ElapsedMilliseconds);

            return CastResult<TResult>(result, queryName, "query");
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
            var invoker = CommandInvokers.GetOrAdd(
                (commandType, typeof(TResult)),
                static key => BuildCommandInvoker(key.RequestType, key.ResultType));

            var result = await invoker(handler, command, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("✅ Command {CommandName} completed in {ElapsedMs}ms",
                commandName, stopwatch.ElapsedMilliseconds);

            return CastResult<TResult>(result, commandName, "command");
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
            var handlerServiceType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(handlerServiceType);
            var handlers = _serviceProvider.GetService(enumerableType) as System.Collections.IEnumerable;

            if (handlers != null)
            {
                var invoker = DomainEventInvokers.GetOrAdd(
                    eventType,
                    static type => BuildDomainEventInvoker(type));

                foreach (var handler in handlers.Cast<object>())
                {
                    await invoker(handler, domainEvent, cancellationToken);
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

    private static TResult CastResult<TResult>(object? result, string requestName, string requestType)
    {
        if (result is null)
        {
            if (default(TResult) is null)
            {
                return default!;
            }

            throw new InvalidOperationException(
                $"❌ Handler for {requestType} '{requestName}' returned null for non-nullable result type.");
        }

        if (result is TResult typedResult)
        {
            return typedResult;
        }

        throw new InvalidOperationException(
            $"❌ Handler for {requestType} '{requestName}' returned an unexpected result type.");
    }

    private static Func<object, object, CancellationToken, Task<object?>> BuildQueryInvoker(Type queryType, Type resultType)
    {
        var method = InvokeQueryHandlerMethod.MakeGenericMethod(queryType, resultType);

        return (handler, query, cancellationToken) =>
            (Task<object?>)method.Invoke(null, [handler, query, cancellationToken])!;
    }

    private static Func<object, object, CancellationToken, Task<object?>> BuildCommandInvoker(Type commandType, Type resultType)
    {
        var method = InvokeCommandHandlerMethod.MakeGenericMethod(commandType, resultType);

        return (handler, command, cancellationToken) =>
            (Task<object?>)method.Invoke(null, [handler, command, cancellationToken])!;
    }

    private static Func<object, IDomainEvent, CancellationToken, Task> BuildDomainEventInvoker(Type eventType)
    {
        var method = InvokeDomainEventHandlerMethod.MakeGenericMethod(eventType);

        return (handler, domainEvent, cancellationToken) =>
            (Task)method.Invoke(null, [handler, domainEvent, cancellationToken])!;
    }

    private static async Task<object?> InvokeQueryHandlerAsync<TQuery, TResult>(
        object handler,
        object query,
        CancellationToken cancellationToken)
        where TQuery : IQuery<TResult>
    {
        return await ((IQueryHandler<TQuery, TResult>)handler)
            .HandleAsync((TQuery)query, cancellationToken);
    }

    private static async Task<object?> InvokeCommandHandlerAsync<TCommand, TResult>(
        object handler,
        object command,
        CancellationToken cancellationToken)
        where TCommand : ICommand<TResult>
    {
        return await ((ICommandHandler<TCommand, TResult>)handler)
            .HandleAsync((TCommand)command, cancellationToken);
    }

    private static Task InvokeDomainEventHandlerAsync<TDomainEvent>(
        object handler,
        IDomainEvent domainEvent,
        CancellationToken cancellationToken)
        where TDomainEvent : IDomainEvent
    {
        return ((IDomainEventHandler<TDomainEvent>)handler)
            .HandleAsync((TDomainEvent)domainEvent, cancellationToken);
    }

    /// <summary>
    /// Automatic FluentValidation integration
    /// </summary>
    private async Task ValidateAsync<T>(T request, CancellationToken cancellationToken)
    {
        var validators = new List<IValidator>();

        var declaredValidatorServiceType = typeof(IEnumerable<>).MakeGenericType(typeof(IValidator<>).MakeGenericType(typeof(T)));
        var declaredValidators = _serviceProvider.GetService(declaredValidatorServiceType) as System.Collections.IEnumerable;
        if (declaredValidators != null)
        {
            validators.AddRange(declaredValidators.Cast<IValidator>());
        }

        var concreteType = request?.GetType();
        if (concreteType != null)
        {
            var concreteValidatorType = typeof(IValidator<>).MakeGenericType(concreteType);
            var concreteValidators = _serviceProvider
                .GetServices(concreteValidatorType)
                .Cast<IValidator>();
            validators.AddRange(concreteValidators);
        }

        var uniqueValidators = validators.Distinct().ToList();
        if (!uniqueValidators.Any())
            return;

        var context = new ValidationContext<object>(request!);
        var validationResults = await Task.WhenAll(
            uniqueValidators.Select(v => v.ValidateAsync(context, cancellationToken)));

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
