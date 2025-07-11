using ProductCatalog.Domain.Common;

namespace ProductCatalog.Domain.Entities;

/// <summary>
/// Category entity
/// </summary>
public class Category : BaseEntity
{
    public string CategoryId { get; private set; } = string.Empty;
    public string CategoryName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public byte[]? Picture { get; private set; }
    public string? PictureLink { get; private set; }

    // Navigation properties
    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    // Private constructor for EF Core
    private Category() { }

    public Category(string categoryId, string categoryName, string? description = null, byte[]? picture = null, string? pictureLink = null)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentException("Category ID cannot be empty", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name cannot be empty", nameof(categoryName));

        CategoryId = categoryId;
        CategoryName = categoryName;
        Description = description;
        Picture = picture;
        PictureLink = pictureLink;
    }

    public void UpdateCategory(string categoryName, string? description = null, byte[]? picture = null, string? pictureLink = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name cannot be empty", nameof(categoryName));

        CategoryName = categoryName;
        Description = description;
        Picture = picture;
        PictureLink = pictureLink;
        UpdatedAt = DateTime.UtcNow;
    }
}
