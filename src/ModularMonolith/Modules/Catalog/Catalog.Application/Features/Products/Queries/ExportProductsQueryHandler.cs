using System.Globalization;
using BuildingBlocks.Application.CQRS;
using Catalog.Domain.Repositories;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Queries;

/// <summary>
/// Handler for ExportProductsQuery — exports all products to CSV bytes
/// </summary>
public class ExportProductsQueryHandler : IQueryHandler<ExportProductsQuery, byte[]>
{
    private readonly IProductRepository _productRepository;

    public ExportProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<byte[]> HandleAsync(ExportProductsQuery query, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository
            .GetQueryableSet(p => p.Category)
            .OrderBy(p => p.ProductName)
            .Select(p => new
            {
                p.Id,
                p.ProductName,
                p.CategoryId,
                CategoryName    = p.Category.CategoryName,
                p.QuantityPerUnit,
                p.UnitPrice,
                p.UnitsInStock,
                p.UnitsOnOrder,
                p.ReorderLevel,
                p.Discontinued,
                p.CreatedDateTime,
                p.UpdatedDateTime
            })
            .ToListAsync(cancellationToken);

        using var memoryStream = new MemoryStream();
        using var writer       = new StreamWriter(memoryStream);
        using var csv          = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(products);

        await writer.FlushAsync(cancellationToken);
        return memoryStream.ToArray();
    }
}
