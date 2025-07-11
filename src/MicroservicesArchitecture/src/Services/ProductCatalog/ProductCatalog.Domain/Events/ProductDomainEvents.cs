using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Events;

/// <summary>
/// Product created domain event
/// </summary>
public class ProductCreatedDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Product Product { get; }

    public ProductCreatedDomainEvent(Product product)
    {
        Product = product;
    }
}

/// <summary>
/// Product updated domain event
/// </summary>
public class ProductUpdatedDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Product Product { get; }

    public ProductUpdatedDomainEvent(Product product)
    {
        Product = product;
    }
}

/// <summary>
/// Product price changed domain event
/// </summary>
public class ProductPriceChangedDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string ProductId { get; }
    public decimal? OldPrice { get; }
    public decimal? NewPrice { get; }

    public ProductPriceChangedDomainEvent(string productId, decimal? oldPrice, decimal? newPrice)
    {
        ProductId = productId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}

/// <summary>
/// Product stock updated domain event
/// </summary>
public class ProductStockUpdatedDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string ProductId { get; }
    public short OldStock { get; }
    public short NewStock { get; }
    public string Reason { get; }

    public ProductStockUpdatedDomainEvent(string productId, short oldStock, short newStock, string reason = "Update")
    {
        ProductId = productId;
        OldStock = oldStock;
        NewStock = newStock;
        Reason = reason;
    }
}

/// <summary>
/// Product discontinued domain event
/// </summary>
public class ProductDiscontinuedDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string ProductId { get; }
    public string ProductName { get; }

    public ProductDiscontinuedDomainEvent(string productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}

/// <summary>
/// Product restored domain event
/// </summary>
public class ProductRestoredDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string ProductId { get; }
    public string ProductName { get; }

    public ProductRestoredDomainEvent(string productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}
