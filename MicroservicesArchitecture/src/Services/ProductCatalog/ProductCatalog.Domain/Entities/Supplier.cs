using Shared.Common.Domain;

namespace ProductCatalog.Domain.Entities;

/// <summary>
/// Supplier entity - based on MonolithArchitecture
/// </summary>
public class Supplier : AggregateRoot
{
    public string SupplierId { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public string? ContactName { get; private set; }
    public string? ContactTitle { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Region { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public string? Phone { get; private set; }
    public string? Fax { get; private set; }
    public string? HomePage { get; private set; }

    // Navigation properties
    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private Supplier() { } // For EF Core

    public Supplier(
        string supplierId,
        string companyName,
        string? contactName = null,
        string? contactTitle = null,
        string? address = null,
        string? city = null,
        string? region = null,
        string? postalCode = null,
        string? country = null,
        string? phone = null,
        string? fax = null,
        string? homePage = null)
    {
        if (string.IsNullOrWhiteSpace(supplierId))
            throw new ArgumentException("Supplier ID cannot be empty", nameof(supplierId));
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name cannot be empty", nameof(companyName));

        SupplierId = supplierId;
        CompanyName = companyName;
        ContactName = contactName;
        ContactTitle = contactTitle;
        Address = address;
        City = city;
        Region = region;
        PostalCode = postalCode;
        Country = country;
        Phone = phone;
        Fax = fax;
        HomePage = homePage;
    }

    public void UpdateSupplier(
        string companyName,
        string? contactName = null,
        string? contactTitle = null,
        string? address = null,
        string? city = null,
        string? region = null,
        string? postalCode = null,
        string? country = null,
        string? phone = null,
        string? fax = null,
        string? homePage = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name cannot be empty", nameof(companyName));

        CompanyName = companyName;
        ContactName = contactName;
        ContactTitle = contactTitle;
        Address = address;
        City = city;
        Region = region;
        PostalCode = postalCode;
        Country = country;
        Phone = phone;
        Fax = fax;
        HomePage = homePage;
        UpdatedAt = DateTime.UtcNow;
    }
}
