namespace ProductManager.Shared.DTOs.ProductDto;

public class CreateProductDto
{
    public string ProductName { get; set; } = null!;

    public string? SupplierId { get; set; }

    public string? CategoryId { get; set; }

    public string? QuantityPerUnit { get; set; }

    public decimal? UnitPrice { get; set; }

    public short? UnitsInStock { get; set; }

    public short? UnitsOnOrder { get; set; }

    public short? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }
}
