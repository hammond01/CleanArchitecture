using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Events;

namespace ProductCatalog.Domain.Entities;

/// <summary>
/// Product aggregate root
/// </summary>
public class Product : AggregateRoot
{
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public string? SupplierId { get; private set; }
    public string? CategoryId { get; private set; }
    public string? QuantityPerUnit { get; private set; }
    public decimal? UnitPrice { get; private set; }
    public short? UnitsInStock { get; private set; }
    public short? UnitsOnOrder { get; private set; }
    public short? ReorderLevel { get; private set; }
    public bool Discontinued { get; private set; }

    // Navigation properties
    public virtual Supplier? Supplier { get; private set; }
    public virtual Category? Category { get; private set; }

    // Private constructor for EF Core
    private Product() { }

    public Product(
        string productId,
        string productName,
        string? supplierId = null,
        string? categoryId = null,
        string? quantityPerUnit = null,
        decimal? unitPrice = null,
        short? unitsInStock = null,
        short? unitsOnOrder = null,
        short? reorderLevel = null,
        bool discontinued = false)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));

        ProductId = productId;
        ProductName = productName;
        SupplierId = supplierId;
        CategoryId = categoryId;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
        UnitsInStock = unitsInStock;
        UnitsOnOrder = unitsOnOrder;
        ReorderLevel = reorderLevel;
        Discontinued = discontinued;

        AddDomainEvent(new ProductCreatedDomainEvent(this));
    }

    public void UpdateProductInfo(
        string productName,
        string? supplierId = null,
        string? categoryId = null,
        string? quantityPerUnit = null,
        decimal? unitPrice = null,
        short? unitsInStock = null,
        short? unitsOnOrder = null,
        short? reorderLevel = null,
        bool discontinued = false)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));

        var oldPrice = UnitPrice;
        var oldStock = UnitsInStock;

        ProductName = productName;
        SupplierId = supplierId;
        CategoryId = categoryId;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
        UnitsInStock = unitsInStock;
        UnitsOnOrder = unitsOnOrder;
        ReorderLevel = reorderLevel;
        Discontinued = discontinued;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductUpdatedDomainEvent(this));

        if (oldPrice != unitPrice)
        {
            AddDomainEvent(new ProductPriceChangedDomainEvent(ProductId, oldPrice, unitPrice));
        }

        if (oldStock != unitsInStock)
        {
            AddDomainEvent(new ProductStockUpdatedDomainEvent(ProductId, oldStock ?? 0, unitsInStock ?? 0));
        }
    }

    public void UpdateStock(short newStock, string reason = "Manual Update")
    {
        var oldStock = UnitsInStock ?? 0;
        UnitsInStock = newStock;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductStockUpdatedDomainEvent(ProductId, oldStock, newStock, reason));
    }

    public void DiscontinueProduct()
    {
        Discontinued = true;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductDiscontinuedDomainEvent(ProductId, ProductName));
    }

    public void RestoreProduct()
    {
        Discontinued = false;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductRestoredDomainEvent(ProductId, ProductName));
    }
}
