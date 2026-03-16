using BuildingBlocks.Application.CQRS;
using Catalog.Application.DTOs;

namespace Catalog.Application.Features.Products.Queries;

/// <summary>
/// Query to get a product by ID
/// </summary>
public record GetProductByIdQuery(string ProductId) : IQuery<ProductDto?>;
