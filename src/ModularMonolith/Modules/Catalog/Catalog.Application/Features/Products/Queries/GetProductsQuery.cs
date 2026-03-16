using BuildingBlocks.Application.CQRS;
using Catalog.Application.DTOs;

namespace Catalog.Application.Features.Products.Queries;

/// <summary>
/// Query to get all products
/// </summary>
public record GetProductsQuery : IQuery<List<ProductDto>>
{
    public string? CategoryId { get; init; }
    public bool? Discontinued { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
