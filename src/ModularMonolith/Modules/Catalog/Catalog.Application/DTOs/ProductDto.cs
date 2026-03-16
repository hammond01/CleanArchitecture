namespace Catalog.Application.DTOs;

/// <summary>
/// Product Data Transfer Object
/// </summary>
public class ProductDto
{
    public string Id { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string CategoryId { get; set; } = null!;
    public string? CategoryName { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
