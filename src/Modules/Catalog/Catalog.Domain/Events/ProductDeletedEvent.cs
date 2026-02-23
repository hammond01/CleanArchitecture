using BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a product is deleted
/// </summary>
public class ProductDeletedEvent : IDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }
    public DateTimeOffset OccurredOn { get; }

    public ProductDeletedEvent(string productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}
