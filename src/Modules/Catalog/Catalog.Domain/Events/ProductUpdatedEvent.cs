using BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a product is updated
/// </summary>
public class ProductUpdatedEvent : IDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }
    public decimal? UnitPrice { get; }
    public DateTimeOffset OccurredOn { get; }

    public ProductUpdatedEvent(
        string productId,
        string productName,
        decimal? unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}
