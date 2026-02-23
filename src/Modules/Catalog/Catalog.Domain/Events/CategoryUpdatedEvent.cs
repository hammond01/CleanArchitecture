using BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a category is updated
/// </summary>
public class CategoryUpdatedEvent : IDomainEvent
{
    public string CategoryId { get; }
    public string CategoryName { get; }
    public DateTimeOffset OccurredOn { get; }

    public CategoryUpdatedEvent(string categoryId, string categoryName)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}
