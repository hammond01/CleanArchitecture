using Shared.Common.Mediator;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Common.Models;

namespace ProductCatalog.Application.Features.Products.Queries;

/// <summary>
/// Get product by ID query
/// </summary>
public record GetProductByIdQuery : IRequest<ProductDto?>
{
    public string Id { get; init; } = string.Empty;
}

/// <summary>
/// Get all products query
/// </summary>
public record GetAllProductsQuery : IRequest<IEnumerable<ProductSummaryDto>>
{
}

/// <summary>
/// Get products by category query
/// </summary>
public record GetProductsByCategoryQuery : IRequest<IEnumerable<ProductSummaryDto>>
{
    public string CategoryId { get; init; } = string.Empty;
}

/// <summary>
/// Get products by supplier query
/// </summary>
public record GetProductsBySupplierQuery : IRequest<IEnumerable<ProductSummaryDto>>
{
    public string SupplierId { get; init; } = string.Empty;
}

/// <summary>
/// Get active products query
/// </summary>
public record GetActiveProductsQuery : IRequest<IEnumerable<ProductSummaryDto>>
{
}

/// <summary>
/// Get low stock products query
/// </summary>
public record GetLowStockProductsQuery : IRequest<IEnumerable<ProductSummaryDto>>
{
}

/// <summary>
/// Get out of stock products query
/// </summary>
public record GetOutOfStockProductsQuery : IRequest<IEnumerable<ProductSummaryDto>>
{
}

/// <summary>
/// Search products by name query
/// </summary>
public record SearchProductsQuery : IRequest<IEnumerable<ProductSummaryDto>>
{
    public string SearchTerm { get; init; } = string.Empty;
}

/// <summary>
/// Get products by price range query
/// </summary>
public record GetProductsByPriceRangeQuery : IRequest<IEnumerable<ProductSummaryDto>>
{
    public decimal MinPrice { get; init; }
    public decimal MaxPrice { get; init; }
}

/// <summary>
/// Get paged products query
/// </summary>
public record GetPagedProductsQuery : IRequest<PagedResponse<ProductSummaryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? CategoryId { get; init; }
    public string? SupplierId { get; init; }
    public string? SearchTerm { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool? ActiveOnly { get; init; }
}

/// <summary>
/// Check product exists query
/// </summary>
public record CheckProductExistsQuery : IRequest<bool>
{
    public string Id { get; init; } = string.Empty;
}
