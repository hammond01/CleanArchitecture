using MediatR;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Commands;

/// <summary>
/// Update order command
/// </summary>
public class UpdateOrderCommand : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;
    public AddressDto? ShippingAddress { get; set; }
    public string? Notes { get; set; }

    public UpdateOrderCommand() { }

    public UpdateOrderCommand(string orderId, UpdateOrderDto dto)
    {
        OrderId = orderId;
        ShippingAddress = dto.ShippingAddress;
        Notes = dto.Notes;
    }
}

/// <summary>
/// Update order status command
/// </summary>
public class UpdateOrderStatusCommand : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;
    public OrderManagement.Domain.Entities.OrderStatus Status { get; set; }
    public string? Notes { get; set; }

    public UpdateOrderStatusCommand() { }

    public UpdateOrderStatusCommand(string orderId, UpdateOrderStatusDto dto)
    {
        OrderId = orderId;
        Status = dto.Status;
        Notes = dto.Notes;
    }
}

/// <summary>
/// Confirm order command
/// </summary>
public class ConfirmOrderCommand : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;

    public ConfirmOrderCommand() { }

    public ConfirmOrderCommand(string orderId)
    {
        OrderId = orderId;
    }
}

/// <summary>
/// Cancel order command
/// </summary>
public class CancelOrderCommand : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;

    public CancelOrderCommand() { }

    public CancelOrderCommand(string orderId, string reason)
    {
        OrderId = orderId;
        Reason = reason;
    }
}

/// <summary>
/// Ship order command
/// </summary>
public class ShipOrderCommand : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;

    public ShipOrderCommand() { }

    public ShipOrderCommand(string orderId, string trackingNumber)
    {
        OrderId = orderId;
        TrackingNumber = trackingNumber;
    }
}

/// <summary>
/// Deliver order command
/// </summary>
public class DeliverOrderCommand : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;

    public DeliverOrderCommand() { }

    public DeliverOrderCommand(string orderId)
    {
        OrderId = orderId;
    }
}

/// <summary>
/// Add order item command
/// </summary>
public class AddOrderItemCommand : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public MoneyDto UnitPrice { get; set; } = null!;

    public AddOrderItemCommand() { }

    public AddOrderItemCommand(string orderId, AddOrderItemDto dto)
    {
        OrderId = orderId;
        ProductId = dto.ProductId;
        ProductName = dto.ProductName;
        Quantity = dto.Quantity;
        UnitPrice = dto.UnitPrice;
    }
}

/// <summary>
/// Remove order item command
/// </summary>
public class RemoveOrderItemCommand : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;

    public RemoveOrderItemCommand() { }

    public RemoveOrderItemCommand(string orderId, string productId)
    {
        OrderId = orderId;
        ProductId = productId;
    }
}

/// <summary>
/// Update order item command
/// </summary>
public class UpdateOrderItemCommand : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public UpdateOrderItemCommand() { }

    public UpdateOrderItemCommand(string orderId, UpdateOrderItemDto dto)
    {
        OrderId = orderId;
        ProductId = dto.ProductId;
        Quantity = dto.Quantity;
    }
}
