using Shared.Events.Common;

namespace Shared.Events.Products;

/// <summary>
/// Product created event
/// </summary>
public class ProductCreatedEvent : IntegrationEvent
{
    public override string EventType => "ProductCreated";
    public override string Source => "ProductCatalogService";

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public bool Discontinued { get; set; }
}

/// <summary>
/// Product updated event
/// </summary>
public class ProductUpdatedEvent : IntegrationEvent
{
    public override string EventType => "ProductUpdated";
    public override string Source => "ProductCatalogService";

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public bool Discontinued { get; set; }
}

/// <summary>
/// Product deleted event
/// </summary>
public class ProductDeletedEvent : IntegrationEvent
{
    public override string EventType => "ProductDeleted";
    public override string Source => "ProductCatalogService";

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
}

/// <summary>
/// Product stock updated event
/// </summary>
public class ProductStockUpdatedEvent : IntegrationEvent
{
    public override string EventType => "ProductStockUpdated";
    public override string Source => "InventoryService";

    public int ProductId { get; set; }
    public short PreviousStock { get; set; }
    public short NewStock { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Product price changed event
/// </summary>
public class ProductPriceChangedEvent : IntegrationEvent
{
    public override string EventType => "ProductPriceChanged";
    public override string Source => "ProductCatalogService";

    public int ProductId { get; set; }
    public decimal? PreviousPrice { get; set; }
    public decimal? NewPrice { get; set; }
    public string Reason { get; set; } = string.Empty;
}
