using BuildingBlocks.Domain.Entities;
using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

/// <summary>
/// Product entity with rich domain model
/// </summary>
public class Product : Entity<string>
{
    // Private setters to enforce encapsulation
    public string ProductName { get; private set; } = null!;
    public string CategoryId { get; private set; } = null!;
    public string? QuantityPerUnit { get; private set; }
    public decimal? UnitPrice { get; private set; }
    public short? UnitsInStock { get; private set; }
    public short? UnitsOnOrder { get; private set; }
    public short? ReorderLevel { get; private set; }
    public bool Discontinued { get; private set; }

    // Navigation property
    public Category Category { get; private set; } = null!;

    // Private constructor for EF Core
    private Product() { }

    // Factory method for creating a new product
    public static Product Create(
        string productName,
        string categoryId,
        string? quantityPerUnit = null,
        decimal? unitPrice = null,
        short? unitsInStock = null,
        short? unitsOnOrder = null,
        short? reorderLevel = null,
        bool discontinued = false)
    {
        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            ProductName = productName,
            CategoryId = categoryId,
            QuantityPerUnit = quantityPerUnit,
            UnitPrice = unitPrice,
            UnitsInStock = unitsInStock,
            UnitsOnOrder = unitsOnOrder,
            ReorderLevel = reorderLevel,
            Discontinued = discontinued
        };

        product.Validate();
        return product;
    }

    // Business logic methods
    public void UpdateDetails(
        string productName,
        string? quantityPerUnit,
        decimal? unitPrice,
        short? reorderLevel)
    {
        ProductName = productName;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
        ReorderLevel = reorderLevel;

        Validate();
    }

    public void ChangeCategory(string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
        {
            throw new ProductDomainException("Category ID cannot be empty");
        }

        CategoryId = categoryId;
    }

    public void UpdateStock(short unitsInStock, short unitsOnOrder)
    {
        if (unitsInStock < 0)
        {
            throw new ProductDomainException("Units in stock cannot be negative");
        }

        if (unitsOnOrder < 0)
        {
            throw new ProductDomainException("Units on order cannot be negative");
        }

        UnitsInStock = unitsInStock;
        UnitsOnOrder = unitsOnOrder;

        CheckReorderLevel();
    }

    public void Discontinue()
    {
        Discontinued = true;
    }

    public void Reactivate()
    {
        Discontinued = false;
    }

    public bool IsLowStock()
    {
        return UnitsInStock.HasValue && ReorderLevel.HasValue && UnitsInStock <= ReorderLevel;
    }

    public bool IsOutOfStock()
    {
        return !UnitsInStock.HasValue || UnitsInStock == 0;
    }

    // Domain validation
    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(ProductName))
        {
            throw new ProductDomainException("Product name is required");
        }

        if (ProductName.Length > 40)
        {
            throw new ProductDomainException("Product name cannot exceed 40 characters");
        }

        if (string.IsNullOrWhiteSpace(CategoryId))
        {
            throw new ProductDomainException("Category ID is required");
        }

        if (!string.IsNullOrWhiteSpace(QuantityPerUnit) && QuantityPerUnit.Length > 20)
        {
            throw new ProductDomainException("Quantity per unit cannot exceed 20 characters");
        }

        if (UnitPrice.HasValue && UnitPrice < 0)
        {
            throw new ProductDomainException("Unit price cannot be negative");
        }

        if (UnitsInStock.HasValue && UnitsInStock < 0)
        {
            throw new ProductDomainException("Units in stock cannot be negative");
        }

        if (UnitsOnOrder.HasValue && UnitsOnOrder < 0)
        {
            throw new ProductDomainException("Units on order cannot be negative");
        }

        if (ReorderLevel.HasValue && ReorderLevel < 0)
        {
            throw new ProductDomainException("Reorder level cannot be negative");
        }
    }

    private void CheckReorderLevel()
    {
        if (IsLowStock())
        {
            // Domain event would be raised here
            // AddDomainEvent(new ProductLowStockEvent(this));
        }
    }
}
