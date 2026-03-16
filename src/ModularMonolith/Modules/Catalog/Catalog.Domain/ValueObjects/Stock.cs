using Catalog.Domain.Exceptions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Stock value object representing inventory quantities
/// </summary>
public class Stock : IEquatable<Stock>
{
    public short UnitsInStock { get; private set; }
    public short UnitsOnOrder { get; private set; }
    public short ReorderLevel { get; private set; }

    // Private constructor for EF Core
    private Stock() { }

    // Factory method
    public static Stock Create(short unitsInStock, short unitsOnOrder, short reorderLevel)
    {
        if (unitsInStock < 0)
        {
            throw new ProductDomainException("Units in stock cannot be negative");
        }

        if (unitsOnOrder < 0)
        {
            throw new ProductDomainException("Units on order cannot be negative");
        }

        if (reorderLevel < 0)
        {
            throw new ProductDomainException("Reorder level cannot be negative");
        }

        return new Stock
        {
            UnitsInStock = unitsInStock,
            UnitsOnOrder = unitsOnOrder,
            ReorderLevel = reorderLevel
        };
    }

    // Business methods
    public bool IsLowStock()
    {
        return UnitsInStock <= ReorderLevel;
    }

    public bool IsOutOfStock()
    {
        return UnitsInStock == 0;
    }

    public short TotalAvailable()
    {
        return (short)(UnitsInStock + UnitsOnOrder);
    }

    public Stock AddStock(short quantity)
    {
        if (quantity < 0)
        {
            throw new ProductDomainException("Quantity to add cannot be negative");
        }

        return Create((short)(UnitsInStock + quantity), UnitsOnOrder, ReorderLevel);
    }

    public Stock RemoveStock(short quantity)
    {
        if (quantity < 0)
        {
            throw new ProductDomainException("Quantity to remove cannot be negative");
        }

        if (quantity > UnitsInStock)
        {
            throw new ProductDomainException("Cannot remove more stock than available");
        }

        return Create((short)(UnitsInStock - quantity), UnitsOnOrder, ReorderLevel);
    }

    public Stock UpdateOnOrder(short newUnitsOnOrder)
    {
        if (newUnitsOnOrder < 0)
        {
            throw new ProductDomainException("Units on order cannot be negative");
        }

        return Create(UnitsInStock, newUnitsOnOrder, ReorderLevel);
    }

    // Equality implementation
    public bool Equals(Stock? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return UnitsInStock == other.UnitsInStock
            && UnitsOnOrder == other.UnitsOnOrder
            && ReorderLevel == other.ReorderLevel;
    }

    public override bool Equals(object? obj)
    {
        return obj is Stock stock && Equals(stock);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UnitsInStock, UnitsOnOrder, ReorderLevel);
    }

    public static bool operator ==(Stock? left, Stock? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Stock? left, Stock? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"Stock: {UnitsInStock}, On Order: {UnitsOnOrder}, Reorder Level: {ReorderLevel}";
    }
}
