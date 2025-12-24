using ProductManager.Application.Common.Queries;
using ProductManager.Application.Feature.Product.Specifications;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Feature.Product.Queries;

/// <summary>
/// Query to get products with category (using Specification Pattern)
/// ✅ EXAMPLE: Shows how to use specification instead of auto-include
/// </summary>
public record GetProductsWithCategory : IQuery<ApiResponse>;

/// <summary>
/// Handler using Specification Pattern for controlled eager loading
/// ✅ PERFORMANCE: Only loads Category when needed, no auto-include
/// </summary>
internal class GetProductsWithCategoryHandler : IQueryHandler<GetProductsWithCategory, ApiResponse>
{
    private readonly IRepository<Products, string> _repository;

    public GetProductsWithCategoryHandler(IRepository<Products, string> repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse> HandleAsync(GetProductsWithCategory query, CancellationToken cancellationToken = default)
    {
        // ✅ Use specification for controlled eager loading
        var spec = new ProductsWithCategorySpecification();
        var products = await _repository.ListAsync(spec, cancellationToken);

        return new ApiResponse(200, "Get products with category successfully", products);
    }
}

/// <summary>
/// Query to search products with pagination
/// ✅ EXAMPLE: Shows specification with filtering + pagination
/// </summary>
public record SearchProductsQuery : IQuery<ApiResponse>
{
    public string SearchTerm { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

/// <summary>
/// Handler for product search with pagination
/// ✅ PERFORMANCE: Efficient pagination with controlled includes
/// </summary>
internal class SearchProductsHandler : IQueryHandler<SearchProductsQuery, ApiResponse>
{
    private readonly IRepository<Products, string> _repository;

    public SearchProductsHandler(IRepository<Products, string> repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse> HandleAsync(SearchProductsQuery query, CancellationToken cancellationToken = default)
    {
        // ✅ Use specification for search + pagination
        var spec = new ProductSearchSpecification(query.SearchTerm, query.PageNumber, query.PageSize);

        var products = await _repository.ListAsync(spec, cancellationToken);
        var totalCount = await _repository.CountAsync(
            new ProductSearchSpecification(query.SearchTerm, 1, int.MaxValue),
            cancellationToken
        );

        return new ApiResponse(200, "Search completed successfully", new
        {
            Items = products,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        });
    }
}

/// <summary>
/// Query to get low stock products
/// ✅ EXAMPLE: Shows business logic in specification
/// </summary>
public record GetLowStockProducts : IQuery<ApiResponse>
{
    public short Threshold { get; init; } = 10;
}

/// <summary>
/// Handler for low stock products
/// ✅ BUSINESS LOGIC: Encapsulated in specification
/// </summary>
internal class GetLowStockProductsHandler : IQueryHandler<GetLowStockProducts, ApiResponse>
{
    private readonly IRepository<Products, string> _repository;

    public GetLowStockProductsHandler(IRepository<Products, string> repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse> HandleAsync(GetLowStockProducts query, CancellationToken cancellationToken = default)
    {
        // ✅ Use specification with business logic
        var spec = new LowStockProductsSpecification(query.Threshold);
        var lowStockProducts = await _repository.ListAsync(spec, cancellationToken);

        return new ApiResponse(200, "Low stock products retrieved successfully", lowStockProducts);
    }
}
