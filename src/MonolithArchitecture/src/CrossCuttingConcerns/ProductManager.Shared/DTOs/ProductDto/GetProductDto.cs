namespace ProductManager.Shared.DTOs.ProductDto;

public class GetProductDto
{
    public string Id { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;
    public string? CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? QuantityPerUnit { get; set; }

    public decimal? UnitPrice { get; set; }

    public short? UnitsInStock { get; set; }

    public short? UnitsOnOrder { get; set; }

    public short? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }
}