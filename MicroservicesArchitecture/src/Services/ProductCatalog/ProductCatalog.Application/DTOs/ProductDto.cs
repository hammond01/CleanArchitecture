namespace ProductCatalog.Application.DTOs;

/// <summary>
/// Product DTO
/// </summary>
public class ProductDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Create product DTO
/// </summary>
public class CreateProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public string? CategoryId { get; set; }
    public string? SupplierId { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
}

/// <summary>
/// Update product DTO
/// </summary>
public class UpdateProductDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? CategoryId { get; set; }
    public string? SupplierId { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
}

/// <summary>
/// Category DTO
/// </summary>
public class CategoryDto
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PictureLink { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Supplier DTO
/// </summary>
public class SupplierDto
{
    public string SupplierId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? HomePage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
