namespace ProductManager.Shared.DTOs.ReportDto;

public class InventoryStatusDto
{
    public int TotalProducts { get; set; }
    public int LowStockThreshold { get; set; }
    public int LowStockProductsCount { get; set; }
    public List<LowStockProductDto> LowStockProducts { get; set; } = new();
    public List<CategoryStockDto> CategoryStockSummary { get; set; } = new();
}

public class LowStockProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public short UnitsInStock { get; set; }
    public short ReorderLevel { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
}

public class CategoryStockDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public int TotalStock { get; set; }
    public decimal TotalValue { get; set; }
}
