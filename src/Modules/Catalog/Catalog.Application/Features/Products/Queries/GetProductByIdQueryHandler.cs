using BuildingBlocks.Application.CQRS;
using Catalog.Application.DTOs;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Queries;

/// <summary>
/// Handler for GetProductByIdQuery
/// </summary>
public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository
            .GetQueryableSet(p => p.Category)
            .Where(p => p.Id == query.ProductId)
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
            .FirstOrDefaultAsync(cancellationToken);

        return product;
    }
}
