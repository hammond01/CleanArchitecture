using OrderManagement.Domain.Events;
using OrderManagement.Domain.ValueObjects;
using Shared.Common.Domain;

namespace OrderManagement.Domain.Entities;

/// <summary>
/// Order aggregate root
/// </summary>
public class Order : AggregateRoot
{
    private readonly List<OrderItem> _orderItems = new();

    public string OrderNumber { get; private set; } = string.Empty;
    public string CustomerId { get; private set; } = string.Empty;
    public CustomerInfo CustomerInfo { get; private set; } = null!;
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public Address? ShippingAddress { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    // Private constructor for EF Core
    private Order() : base() { }

    // Factory method for creating new orders
    public static Order Create(
        string customerId,
        CustomerInfo customerInfo,
        Address? shippingAddress = null,
        string? notes = null)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            CustomerInfo = customerInfo,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddress = shippingAddress,
            Notes = notes,
            TotalAmount = Money.Zero()
        };

        order.AddDomainEvent(new OrderCreatedEvent(order.Number, order.OrderNumber, customerId));
        return order;
    }

    public void AddOrderItem(string productId, string productName, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify order items when order is not in pending status");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        var existingItem = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = OrderItem.Create(productId, productName, quantity, unitPrice);
            _orderItems.Add(orderItem);
        }

        RecalculateTotalAmount();
        AddDomainEvent(new OrderItemAddedEvent(Number, productId, quantity));
    }

    public void RemoveOrderItem(string productId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify order items when order is not in pending status");

        var item = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            _orderItems.Remove(item);
            RecalculateTotalAmount();
            AddDomainEvent(new OrderItemRemovedEvent(Number, productId));
        }
    }

    public void UpdateOrderItemQuantity(string productId, int newQuantity)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify order items when order is not in pending status");

        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        var item = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        if (item == null)
            throw new ArgumentException($"Order item with product ID {productId} not found");

        item.UpdateQuantity(newQuantity);
        RecalculateTotalAmount();
        AddDomainEvent(new OrderItemQuantityUpdatedEvent(Number, productId, newQuantity));
    }

    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        if (!_orderItems.Any())
            throw new InvalidOperationException("Cannot confirm order without items");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(Number, OrderNumber, CustomerId, TotalAmount.Amount));
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be processed");

        Status = OrderStatus.Processing;
        AddDomainEvent(new OrderProcessingStartedEvent(Number, OrderNumber));
    }

    public void Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Only processing orders can be shipped");

        Status = OrderStatus.Shipped;
        AddDomainEvent(new OrderShippedEvent(Number, OrderNumber, trackingNumber));
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered");

        Status = OrderStatus.Delivered;
        AddDomainEvent(new OrderDeliveredEvent(Number, OrderNumber));
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel delivered orders");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Order is already cancelled");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Number, OrderNumber, reason));
    }

    public void UpdateShippingAddress(Address newAddress)
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Cannot update shipping address for orders in processing or later stages");

        ShippingAddress = newAddress;
        AddDomainEvent(new OrderShippingAddressUpdatedEvent(Number, newAddress));
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }

    private void RecalculateTotalAmount()
    {
        var total = _orderItems.Sum(item => item.TotalPrice.Amount);
        TotalAmount = Money.Create(total, TotalAmount.Currency);
    }

    private static string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Random.Shared.Next(1000, 9999);
        return $"ORD-{timestamp}-{random}";
    }
}
