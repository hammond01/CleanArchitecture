using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Events;

namespace ProductCatalog.Domain.Entities;

/// <summary>
/// Category entity
/// </summary>
public class Category : BaseEntity
{
    public string CategoryName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    // Navigation properties
    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { } // For EF Core

    public Category(string categoryName, string? description = null)
    {
        CategoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
        Description = description;
        IsActive = true;
        DisplayOrder = 0;

        AddDomainEvent(new CategoryCreatedEvent(Id, CategoryName));
    }

    public void UpdateInfo(string categoryName, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name cannot be empty", nameof(categoryName));

        CategoryName = categoryName;
        Description = description;
    }

    public void SetImageUrl(string? imageUrl)
    {
        ImageUrl = imageUrl;
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative", nameof(displayOrder));

        DisplayOrder = displayOrder;
    }

    public void Activate()
    {
        IsActive = true;
        AddDomainEvent(new CategoryActivatedEvent(Id, CategoryName));
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new CategoryDeactivatedEvent(Id, CategoryName));
    }
}

/// <summary>
/// Supplier entity
/// </summary>
public class Supplier : BaseEntity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string? ContactName { get; private set; }
    public string? ContactTitle { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Region { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }
    public bool IsActive { get; private set; }
    public SupplierRating Rating { get; private set; }

    // Navigation properties
    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Supplier() { } // For EF Core

    public Supplier(
        string companyName,
        string? contactName = null,
        string? email = null,
        string? phone = null)
    {
        CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
        ContactName = contactName;
        Email = email;
        Phone = phone;
        IsActive = true;
        Rating = SupplierRating.Unrated;

        AddDomainEvent(new SupplierCreatedEvent(Id, CompanyName));
    }

    public void UpdateContactInfo(
        string? contactName = null,
        string? contactTitle = null,
        string? email = null,
        string? phone = null,
        string? website = null)
    {
        ContactName = contactName;
        ContactTitle = contactTitle;
        Email = email;
        Phone = phone;
        Website = website;
    }

    public void UpdateAddress(
        string? address = null,
        string? city = null,
        string? region = null,
        string? postalCode = null,
        string? country = null)
    {
        Address = address;
        City = city;
        Region = region;
        PostalCode = postalCode;
        Country = country;
    }

    public void SetRating(SupplierRating rating)
    {
        var oldRating = Rating;
        Rating = rating;

        if (oldRating != rating)
        {
            AddDomainEvent(new SupplierRatingChangedEvent(Id, CompanyName, oldRating, rating));
        }
    }

    public void Activate()
    {
        IsActive = true;
        AddDomainEvent(new SupplierActivatedEvent(Id, CompanyName));
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new SupplierDeactivatedEvent(Id, CompanyName));
    }
}

/// <summary>
/// Supplier rating enumeration
/// </summary>
public enum SupplierRating
{
    Unrated = 0,
    Poor = 1,
    Fair = 2,
    Good = 3,
    VeryGood = 4,
    Excellent = 5
}
