using BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a new category is created
/// </summary>
public class CategoryCreatedEvent : IDomainEvent
{
    public string CategoryId { get; }
    public string CategoryName { get; }
    public DateTimeOffset OccurredOn { get; }

    public CategoryCreatedEvent(string categoryId, string categoryName)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}
