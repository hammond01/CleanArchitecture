using BuildingBlocks.Application.CQRS;

namespace Catalog.Application.Features.Products.Queries;

/// <summary>
/// Query to export all products as CSV byte array
/// </summary>
public record ExportProductsQuery : IQuery<byte[]>;
