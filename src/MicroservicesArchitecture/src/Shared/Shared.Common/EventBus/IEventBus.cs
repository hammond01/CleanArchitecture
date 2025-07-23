using Shared.Events.Common;

namespace Shared.Common.EventBus;

/// <summary>
/// Event bus interface for publishing and subscribing to events
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publish an integration event
    /// </summary>
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent;

    /// <summary>
    /// Subscribe to an integration event
    /// </summary>
    Task SubscribeAsync<T, TH>(CancellationToken cancellationToken = default)
        where T : IntegrationEvent
        where TH : class, IIntegrationEventHandler<T>;

    /// <summary>
    /// Unsubscribe from an integration event
    /// </summary>
    Task UnsubscribeAsync<T, TH>(CancellationToken cancellationToken = default)
        where T : IntegrationEvent
        where TH : class, IIntegrationEventHandler<T>;
}

/// <summary>
/// Integration event handler interface
/// </summary>
public interface IIntegrationEventHandler<in T> where T : IntegrationEvent
{
    Task HandleAsync(T @event, CancellationToken cancellationToken = default);
}

/// <summary>
/// Event bus subscription manager interface
/// </summary>
public interface IEventBusSubscriptionManager
{
    bool IsEmpty { get; }
    
    event EventHandler<string> OnEventRemoved;
    
    void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : class, IIntegrationEventHandler<T>;
    
    void RemoveSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : class, IIntegrationEventHandler<T>;
    
    bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
    bool HasSubscriptionsForEvent(string eventName);
    
    Type? GetEventTypeByName(string eventName);
    
    void Clear();
    
    IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
    
    string GetEventKey<T>() where T : IntegrationEvent;
}

/// <summary>
/// Subscription information
/// </summary>
public class SubscriptionInfo
{
    public Type HandlerType { get; }
    public bool IsDynamic { get; }

    private SubscriptionInfo(bool isDynamic, Type handlerType)
    {
        IsDynamic = isDynamic;
        HandlerType = handlerType;
    }

    public static SubscriptionInfo Dynamic(Type handlerType)
    {
        return new SubscriptionInfo(true, handlerType);
    }

    public static SubscriptionInfo Typed(Type handlerType)
    {
        return new SubscriptionInfo(false, handlerType);
    }
}
