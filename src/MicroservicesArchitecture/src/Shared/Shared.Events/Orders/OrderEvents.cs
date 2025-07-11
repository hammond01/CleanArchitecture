using Shared.Events.Common;

namespace Shared.Events.Orders;

/// <summary>
/// Order created event
/// </summary>
public class OrderCreatedEvent : IntegrationEvent
{
    public override string EventType => "OrderCreated";
    public override string Source => "OrderManagementService";

    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public decimal? Freight { get; set; }
    public List<OrderDetailInfo> OrderDetails { get; set; } = new();
}

/// <summary>
/// Order updated event
/// </summary>
public class OrderUpdatedEvent : IntegrationEvent
{
    public override string EventType => "OrderUpdated";
    public override string Source => "OrderManagementService";

    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public decimal? Freight { get; set; }
}

/// <summary>
/// Order shipped event
/// </summary>
public class OrderShippedEvent : IntegrationEvent
{
    public override string EventType => "OrderShipped";
    public override string Source => "OrderManagementService";

    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public DateTime ShippedDate { get; set; }
    public int? ShipVia { get; set; }
    public string? TrackingNumber { get; set; }
    public List<OrderDetailInfo> OrderDetails { get; set; } = new();
}

/// <summary>
/// Order cancelled event
/// </summary>
public class OrderCancelledEvent : IntegrationEvent
{
    public override string EventType => "OrderCancelled";
    public override string Source => "OrderManagementService";

    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public DateTime CancelledDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public List<OrderDetailInfo> OrderDetails { get; set; } = new();
}

/// <summary>
/// Order detail information for events
/// </summary>
public class OrderDetailInfo
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public short Quantity { get; set; }
    public float Discount { get; set; }
}
