using BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

/// <summary>
/// Domain event raised when product stock is low
/// </summary>
public class ProductLowStockEvent : IDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }
    public short CurrentStock { get; }
    public short ReorderLevel { get; }
    public DateTimeOffset OccurredOn { get; }

    public ProductLowStockEvent(
        string productId,
        string productName,
        short currentStock,
        short reorderLevel)
    {
        ProductId = productId;
        ProductName = productName;
        CurrentStock = currentStock;
        ReorderLevel = reorderLevel;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}
