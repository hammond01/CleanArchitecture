using ProductCatalog.Domain.Common;

namespace ProductCatalog.Domain.Entities;

/// <summary>
/// Category entity
/// </summary>
public class Category : BaseEntity
{
    public int CategoryId { get; private set; }
    public string CategoryName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public byte[]? Picture { get; private set; }

    // Navigation properties
    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    // Private constructor for EF Core
    private Category() { }

    public Category(string categoryName, string? description = null, byte[]? picture = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name cannot be empty", nameof(categoryName));

        CategoryName = categoryName;
        Description = description;
        Picture = picture;
    }

    public void UpdateCategory(string categoryName, string? description = null, byte[]? picture = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name cannot be empty", nameof(categoryName));

        CategoryName = categoryName;
        Description = description;
        Picture = picture;
        UpdatedAt = DateTime.UtcNow;
    }
}
