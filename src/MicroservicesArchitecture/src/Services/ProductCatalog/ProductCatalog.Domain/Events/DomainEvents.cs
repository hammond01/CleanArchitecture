using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Events;

// Product Events
public sealed class ProductCreatedEvent : BaseDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }
    public string CategoryId { get; }
    public decimal UnitPrice { get; }

    public ProductCreatedEvent(string productId, string productName, string categoryId, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        CategoryId = categoryId;
        UnitPrice = unitPrice;
    }
}

public sealed class ProductNameChangedEvent : BaseDomainEvent
{
    public string ProductId { get; }
    public string OldName { get; }
    public string NewName { get; }

    public ProductNameChangedEvent(string productId, string oldName, string newName)
    {
        ProductId = productId;
        OldName = oldName;
        NewName = newName;
    }
}

public sealed class ProductPriceChangedEvent : BaseDomainEvent
{
    public string ProductId { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }

    public ProductPriceChangedEvent(string productId, decimal oldPrice, decimal newPrice)
    {
        ProductId = productId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}

public sealed class ProductStockUpdatedEvent : BaseDomainEvent
{
    public string ProductId { get; }
    public int OldStock { get; }
    public int NewStock { get; }

    public ProductStockUpdatedEvent(string productId, int oldStock, int newStock)
    {
        ProductId = productId;
        OldStock = oldStock;
        NewStock = newStock;
    }
}

public sealed class ProductReorderRequiredEvent : BaseDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }
    public int CurrentStock { get; }
    public int ReorderLevel { get; }

    public ProductReorderRequiredEvent(string productId, string productName, int currentStock, int reorderLevel)
    {
        ProductId = productId;
        ProductName = productName;
        CurrentStock = currentStock;
        ReorderLevel = reorderLevel;
    }
}

public sealed class ProductDiscontinuedEvent : BaseDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }

    public ProductDiscontinuedEvent(string productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}

public sealed class ProductActivatedEvent : BaseDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }

    public ProductActivatedEvent(string productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}

// Category Events
public sealed class CategoryCreatedEvent : BaseDomainEvent
{
    public string CategoryId { get; }
    public string CategoryName { get; }

    public CategoryCreatedEvent(string categoryId, string categoryName)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
    }
}

public sealed class CategoryActivatedEvent : BaseDomainEvent
{
    public string CategoryId { get; }
    public string CategoryName { get; }

    public CategoryActivatedEvent(string categoryId, string categoryName)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
    }
}

public sealed class CategoryDeactivatedEvent : BaseDomainEvent
{
    public string CategoryId { get; }
    public string CategoryName { get; }

    public CategoryDeactivatedEvent(string categoryId, string categoryName)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
    }
}

// Supplier Events
public sealed class SupplierCreatedEvent : BaseDomainEvent
{
    public string SupplierId { get; }
    public string CompanyName { get; }

    public SupplierCreatedEvent(string supplierId, string companyName)
    {
        SupplierId = supplierId;
        CompanyName = companyName;
    }
}

public sealed class SupplierRatingChangedEvent : BaseDomainEvent
{
    public string SupplierId { get; }
    public string CompanyName { get; }
    public SupplierRating OldRating { get; }
    public SupplierRating NewRating { get; }

    public SupplierRatingChangedEvent(string supplierId, string companyName, SupplierRating oldRating, SupplierRating newRating)
    {
        SupplierId = supplierId;
        CompanyName = companyName;
        OldRating = oldRating;
        NewRating = newRating;
    }
}

public sealed class SupplierActivatedEvent : BaseDomainEvent
{
    public string SupplierId { get; }
    public string CompanyName { get; }

    public SupplierActivatedEvent(string supplierId, string companyName)
    {
        SupplierId = supplierId;
        CompanyName = companyName;
    }
}

public sealed class SupplierDeactivatedEvent : BaseDomainEvent
{
    public string SupplierId { get; }
    public string CompanyName { get; }

    public SupplierDeactivatedEvent(string supplierId, string companyName)
    {
        SupplierId = supplierId;
        CompanyName = companyName;
    }
}
