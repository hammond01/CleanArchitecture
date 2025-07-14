using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Events;

namespace ProductCatalog.Domain.Entities;

/// <summary>
/// Product aggregate root
/// </summary>
public class Product : BaseEntity
{
    public string ProductName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string CategoryId { get; private set; } = string.Empty;
    public string SupplierId { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int UnitsInStock { get; private set; }
    public int UnitsOnOrder { get; private set; }
    public int ReorderLevel { get; private set; }
    public bool Discontinued { get; private set; }
    public string? ImageUrl { get; private set; }
    public ProductStatus Status { get; private set; }
    public decimal Weight { get; private set; }
    public string? Dimensions { get; private set; }

    // Navigation properties
    public Category? Category { get; private set; }
    public Supplier? Supplier { get; private set; }

    private Product() { } // For EF Core

    public Product(
        string productName,
        string categoryId,
        string supplierId,
        decimal unitPrice,
        int unitsInStock = 0,
        string? description = null)
    {
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        CategoryId = categoryId ?? throw new ArgumentNullException(nameof(categoryId));
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        UnitPrice = unitPrice;
        UnitsInStock = unitsInStock;
        Description = description;
        Status = ProductStatus.Active;
        Discontinued = false;
        ReorderLevel = 10; // Default reorder level

        // Add domain event
        AddDomainEvent(new ProductCreatedEvent(Id, ProductName, CategoryId, UnitPrice));
    }

    public void UpdateProductInfo(string productName, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));

        var oldName = ProductName;
        ProductName = productName;
        Description = description;

        if (oldName != productName)
        {
            AddDomainEvent(new ProductNameChangedEvent(Id, oldName, productName));
        }
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(newPrice));

        var oldPrice = UnitPrice;
        UnitPrice = newPrice;

        AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, newPrice));
    }

    public void UpdateStock(int newStock)
    {
        if (newStock < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(newStock));

        var oldStock = UnitsInStock;
        UnitsInStock = newStock;

        AddDomainEvent(new ProductStockUpdatedEvent(Id, oldStock, newStock));

        // Check if we need to reorder
        if (newStock <= ReorderLevel && !Discontinued)
        {
            AddDomainEvent(new ProductReorderRequiredEvent(Id, ProductName, newStock, ReorderLevel));
        }
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (UnitsInStock < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {UnitsInStock}, Requested: {quantity}");

        UpdateStock(UnitsInStock - quantity);
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        UpdateStock(UnitsInStock + quantity);
    }

    public void Discontinue()
    {
        Discontinued = true;
        Status = ProductStatus.Discontinued;
        AddDomainEvent(new ProductDiscontinuedEvent(Id, ProductName));
    }

    public void Activate()
    {
        Discontinued = false;
        Status = ProductStatus.Active;
        AddDomainEvent(new ProductActivatedEvent(Id, ProductName));
    }

    public void SetReorderLevel(int reorderLevel)
    {
        if (reorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative", nameof(reorderLevel));

        ReorderLevel = reorderLevel;
    }

    public void SetImageUrl(string? imageUrl)
    {
        ImageUrl = imageUrl;
    }

    public void SetWeight(decimal weight)
    {
        if (weight < 0)
            throw new ArgumentException("Weight cannot be negative", nameof(weight));

        Weight = weight;
    }

    public void SetDimensions(string? dimensions)
    {
        Dimensions = dimensions;
    }

    public bool IsLowStock() => UnitsInStock <= ReorderLevel;
    public bool IsOutOfStock() => UnitsInStock == 0;
    public bool IsAvailable() => !Discontinued && UnitsInStock > 0 && Status == ProductStatus.Active;
}

/// <summary>
/// Product status enumeration
/// </summary>
public enum ProductStatus
{
    Active = 1,
    Inactive = 2,
    Discontinued = 3,
    OutOfStock = 4
}
