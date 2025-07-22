using Shared.Common.Domain;

namespace OrderManagement.Domain.Events;

/// <summary>
/// Base class for order domain events
/// </summary>
public abstract class OrderDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string OrderId { get; }

    protected OrderDomainEvent(string orderId)
    {
        OrderId = orderId;
    }
}

/// <summary>
/// Event raised when an order is created
/// </summary>
public class OrderCreatedEvent : OrderDomainEvent
{
    public string OrderNumber { get; }
    public string CustomerId { get; }

    public OrderCreatedEvent(string orderId, string orderNumber, string customerId) 
        : base(orderId)
    {
        OrderNumber = orderNumber;
        CustomerId = customerId;
    }
}

/// <summary>
/// Event raised when an order is confirmed
/// </summary>
public class OrderConfirmedEvent : OrderDomainEvent
{
    public string OrderNumber { get; }
    public string CustomerId { get; }
    public decimal TotalAmount { get; }

    public OrderConfirmedEvent(string orderId, string orderNumber, string customerId, decimal totalAmount) 
        : base(orderId)
    {
        OrderNumber = orderNumber;
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }
}

/// <summary>
/// Event raised when order processing starts
/// </summary>
public class OrderProcessingStartedEvent : OrderDomainEvent
{
    public string OrderNumber { get; }

    public OrderProcessingStartedEvent(string orderId, string orderNumber) 
        : base(orderId)
    {
        OrderNumber = orderNumber;
    }
}

/// <summary>
/// Event raised when an order is shipped
/// </summary>
public class OrderShippedEvent : OrderDomainEvent
{
    public string OrderNumber { get; }
    public string TrackingNumber { get; }

    public OrderShippedEvent(string orderId, string orderNumber, string trackingNumber) 
        : base(orderId)
    {
        OrderNumber = orderNumber;
        TrackingNumber = trackingNumber;
    }
}

/// <summary>
/// Event raised when an order is delivered
/// </summary>
public class OrderDeliveredEvent : OrderDomainEvent
{
    public string OrderNumber { get; }

    public OrderDeliveredEvent(string orderId, string orderNumber) 
        : base(orderId)
    {
        OrderNumber = orderNumber;
    }
}

/// <summary>
/// Event raised when an order is cancelled
/// </summary>
public class OrderCancelledEvent : OrderDomainEvent
{
    public string OrderNumber { get; }
    public string Reason { get; }

    public OrderCancelledEvent(string orderId, string orderNumber, string reason) 
        : base(orderId)
    {
        OrderNumber = orderNumber;
        Reason = reason;
    }
}

/// <summary>
/// Event raised when an order item is added
/// </summary>
public class OrderItemAddedEvent : OrderDomainEvent
{
    public string ProductId { get; }
    public int Quantity { get; }

    public OrderItemAddedEvent(string orderId, string productId, int quantity) 
        : base(orderId)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}

/// <summary>
/// Event raised when an order item is removed
/// </summary>
public class OrderItemRemovedEvent : OrderDomainEvent
{
    public string ProductId { get; }

    public OrderItemRemovedEvent(string orderId, string productId) 
        : base(orderId)
    {
        ProductId = productId;
    }
}

/// <summary>
/// Event raised when order item quantity is updated
/// </summary>
public class OrderItemQuantityUpdatedEvent : OrderDomainEvent
{
    public string ProductId { get; }
    public int NewQuantity { get; }

    public OrderItemQuantityUpdatedEvent(string orderId, string productId, int newQuantity) 
        : base(orderId)
    {
        ProductId = productId;
        NewQuantity = newQuantity;
    }
}

/// <summary>
/// Event raised when order shipping address is updated
/// </summary>
public class OrderShippingAddressUpdatedEvent : OrderDomainEvent
{
    public OrderManagement.Domain.ValueObjects.Address NewAddress { get; }

    public OrderShippingAddressUpdatedEvent(string orderId, OrderManagement.Domain.ValueObjects.Address newAddress) 
        : base(orderId)
    {
        NewAddress = newAddress;
    }
}
