using OrderManagement.Domain.ValueObjects;
using Shared.Common.Domain;

namespace OrderManagement.Domain.Entities;

/// <summary>
/// Order item entity
/// </summary>
public class OrderItem : BaseEntity
{
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public Money TotalPrice { get; private set; } = null!;

    // Private constructor for EF Core
    private OrderItem() : base() { }

    // Factory method for creating order items
    public static OrderItem Create(string productId, string productName, int quantity, Money unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be null or empty", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice.Amount <= 0)
            throw new ArgumentException("Unit price must be greater than zero", nameof(unitPrice));

        var orderItem = new OrderItem
        {
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        orderItem.CalculateTotalPrice();
        return orderItem;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
        CalculateTotalPrice();
    }

    public void UpdateUnitPrice(Money newUnitPrice)
    {
        if (newUnitPrice.Amount <= 0)
            throw new ArgumentException("Unit price must be greater than zero", nameof(newUnitPrice));

        UnitPrice = newUnitPrice;
        CalculateTotalPrice();
    }

    private void CalculateTotalPrice()
    {
        TotalPrice = Money.Create(UnitPrice.Amount * Quantity, UnitPrice.Currency);
    }
}
