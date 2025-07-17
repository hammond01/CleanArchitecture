using Shared.Common.Domain;

namespace ProductCatalog.Domain.Entities;

/// <summary>
/// Product aggregate root - based on MonolithArchitecture
/// </summary>
public class Product : AggregateRoot
{
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public string? CategoryId { get; private set; }
    public string? SupplierId { get; private set; }
    public string? QuantityPerUnit { get; private set; }
    public decimal? UnitPrice { get; private set; }
    public short? UnitsInStock { get; private set; }
    public short? UnitsOnOrder { get; private set; }
    public short? ReorderLevel { get; private set; }
    public bool Discontinued { get; private set; }

    // Navigation properties
    public Category? Category { get; private set; }
    public Supplier? Supplier { get; private set; }

    private Product() { } // For EF Core

    public Product(
        string productId,
        string productName,
        string? categoryId = null,
        string? supplierId = null,
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
        CategoryId = categoryId;
        SupplierId = supplierId;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
        UnitsInStock = unitsInStock;
        UnitsOnOrder = unitsOnOrder;
        ReorderLevel = reorderLevel;
        Discontinued = discontinued;
    }

    public void UpdateProduct(
        string productName,
        string? categoryId = null,
        string? supplierId = null,
        string? quantityPerUnit = null,
        decimal? unitPrice = null,
        short? unitsInStock = null,
        short? unitsOnOrder = null,
        short? reorderLevel = null,
        bool discontinued = false)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));

        ProductName = productName;
        CategoryId = categoryId;
        SupplierId = supplierId;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
        UnitsInStock = unitsInStock;
        UnitsOnOrder = unitsOnOrder;
        ReorderLevel = reorderLevel;
        Discontinued = discontinued;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal? newPrice)
    {
        UnitPrice = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStock(short? newStock)
    {
        UnitsInStock = newStock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Discontinue()
    {
        Discontinued = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Discontinued = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLowStock() => UnitsInStock.HasValue && ReorderLevel.HasValue && UnitsInStock <= ReorderLevel;
    public bool IsOutOfStock() => UnitsInStock == 0;
    public bool IsAvailable() => !Discontinued && UnitsInStock > 0;
}
