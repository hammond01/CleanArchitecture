using BuildingBlocks.Domain.Entities;

namespace Catalog.Domain.Entities;

/// <summary>
/// Product entity
/// </summary>
public class Product : Entity<string>
{
    public string ProductName { get; set; } = null!;
    public string CategoryId { get; set; } = null!;
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }

    // Navigation property
    public Category Category { get; set; } = null!;
}
