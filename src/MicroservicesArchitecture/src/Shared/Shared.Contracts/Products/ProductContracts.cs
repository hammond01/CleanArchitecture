using Shared.Contracts.Common;

namespace Shared.Contracts.Products;

/// <summary>
/// Product data transfer object
/// </summary>
public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
    public string? SupplierName { get; set; }
    public string? CategoryName { get; set; }
}

/// <summary>
/// Create product request
/// </summary>
public class CreateProductRequest : IRequest<ApiResponse<ProductDto>>
{
    public string ProductName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
}

/// <summary>
/// Update product request
/// </summary>
public class UpdateProductRequest : IRequest<ApiResponse<ProductDto>>
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
}

/// <summary>
/// Get product by ID request
/// </summary>
public class GetProductByIdRequest : IRequest<ApiResponse<ProductDto>>
{
    public int ProductId { get; set; }
}

/// <summary>
/// Get products request
/// </summary>
public class GetProductsRequest : IRequest<ApiResponse<List<ProductDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }
    public bool? Discontinued { get; set; }
}

/// <summary>
/// Delete product request
/// </summary>
public class DeleteProductRequest : IRequest<ApiResponse>
{
    public int ProductId { get; set; }
}
