using Shared.Common.Domain;

namespace ProductCatalog.Domain.Entities;

/// <summary>
/// Category entity - based on MonolithArchitecture
/// </summary>
public class Category : AggregateRoot
{
    public string CategoryId { get; private set; } = string.Empty;
    public string CategoryName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? PictureLink { get; private set; }

    // Navigation properties
    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { } // For EF Core

    public Category(string categoryId, string categoryName, string? description = null, string? pictureLink = null)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentException("Category ID cannot be empty", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name cannot be empty", nameof(categoryName));

        CategoryId = categoryId;
        CategoryName = categoryName;
        Description = description;
        PictureLink = pictureLink;
    }

    public void UpdateCategory(string categoryName, string? description = null, string? pictureLink = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name cannot be empty", nameof(categoryName));

        CategoryName = categoryName;
        Description = description;
        PictureLink = pictureLink;
        UpdatedAt = DateTime.UtcNow;
    }
}
