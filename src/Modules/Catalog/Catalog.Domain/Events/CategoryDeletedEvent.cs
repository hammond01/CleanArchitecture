using BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a category is deleted
/// </summary>
public class CategoryDeletedEvent : IDomainEvent
{
    public string CategoryId { get; }
    public string CategoryName { get; }
    public DateTimeOffset OccurredOn { get; }

    public CategoryDeletedEvent(string categoryId, string categoryName)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}
