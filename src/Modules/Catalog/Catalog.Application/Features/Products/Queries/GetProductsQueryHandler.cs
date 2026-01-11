using BuildingBlocks.Application.CQRS;
using Catalog.Application.DTOs;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Queries;

/// <summary>
/// Handler for GetProductsQuery
/// </summary>
public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken = default)
    {
        var productsQuery = _productRepository.GetQueryableSet(p => p.Category);

        // Apply filters
        if (!string.IsNullOrEmpty(query.CategoryId))
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == query.CategoryId);
        }

        if (query.Discontinued.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.Discontinued == query.Discontinued.Value);
        }

        // Apply pagination
        var products = await productsQuery
            .OrderBy(p => p.ProductName)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                QuantityPerUnit = p.QuantityPerUnit,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                UnitsOnOrder = p.UnitsOnOrder,
                ReorderLevel = p.ReorderLevel,
                Discontinued = p.Discontinued,
                CreatedDateTime = p.CreatedDateTime,
                UpdatedDateTime = p.UpdatedDateTime
            })
            .ToListAsync(cancellationToken);

        return products;
    }
}
