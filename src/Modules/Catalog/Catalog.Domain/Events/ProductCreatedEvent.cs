using BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a new product is created
/// </summary>
public class ProductCreatedEvent : IDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }
    public string CategoryId { get; }
    public decimal? UnitPrice { get; }
    public DateTimeOffset OccurredOn { get; }

    public ProductCreatedEvent(
        string productId,
        string productName,
        string categoryId,
        decimal? unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        CategoryId = categoryId;
        UnitPrice = unitPrice;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}
