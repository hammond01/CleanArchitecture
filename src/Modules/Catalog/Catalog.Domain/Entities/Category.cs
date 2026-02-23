using BuildingBlocks.Domain.Entities;
using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

/// <summary>
/// Category entity with rich domain model
/// </summary>
public class Category : Entity<string>
{
    // Private setters to enforce encapsulation
    public string CategoryName { get; private set; } = null!;
    public string? Description { get; private set; }
    public byte[]? Picture { get; private set; }
    public string? PictureLink { get; private set; }

    // Navigation property with private setter
    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    // Private constructor for EF Core
    private Category() { }

    // Factory method for creating a new category
    public static Category Create(
        string categoryName,
        string? description = null,
        string? pictureLink = null)
    {
        var category = new Category
        {
            Id = Guid.NewGuid().ToString(),
            CategoryName = categoryName,
            Description = description,
            PictureLink = pictureLink
        };

        category.Validate();
        return category;
    }

    // Business logic methods
    public void UpdateDetails(string categoryName, string? description)
    {
        CategoryName = categoryName;
        Description = description;

        Validate();
    }

    public void SetPicture(byte[] picture)
    {
        if (picture == null || picture.Length == 0)
        {
            throw new CategoryDomainException("Picture cannot be empty");
        }

        if (picture.Length > 1024 * 1024 * 5) // 5MB limit
        {
            throw new CategoryDomainException("Picture size cannot exceed 5MB");
        }

        Picture = picture;
    }

    public void SetPictureLink(string? pictureLink)
    {
        if (!string.IsNullOrWhiteSpace(pictureLink) && !Uri.IsWellFormedUriString(pictureLink, UriKind.Absolute))
        {
            throw new CategoryDomainException("Picture link must be a valid URL");
        }

        PictureLink = pictureLink;
    }

    public void RemovePicture()
    {
        Picture = null;
        PictureLink = null;
    }

    public bool HasProducts()
    {
        return _products.Any();
    }

    public int GetProductCount()
    {
        return _products.Count;
    }

    // Domain validation
    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(CategoryName))
        {
            throw new CategoryDomainException("Category name is required");
        }

        if (CategoryName.Length > 100)
        {
            throw new CategoryDomainException("Category name cannot exceed 100 characters");
        }

        if (!string.IsNullOrWhiteSpace(Description) && Description.Length > 500)
        {
            throw new CategoryDomainException("Description cannot exceed 500 characters");
        }
    }

    // Internal method for EF Core to add products
    internal void AddProduct(Product product)
    {
        if (product == null)
        {
            throw new CategoryDomainException("Product cannot be null");
        }

        if (_products.Any(p => p.Id == product.Id))
        {
            return; // Already exists
        }

        _products.Add(product);
    }
}
