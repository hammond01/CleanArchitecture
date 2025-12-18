using System.Globalization;
using System.IO;
using CsvHelper;
using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Product.Queries;

public record ExportProducts : IQuery<byte[]>;
internal class ExportProductsHandler : IQueryHandler<ExportProducts, byte[]>
{
    private readonly ICrudService<Products> _crudService;

    public ExportProductsHandler(ICrudService<Products> crudService)
    {
        _crudService = crudService;
    }

    public async Task<byte[]> HandleAsync(ExportProducts query, CancellationToken cancellationToken = default)
    {
        var products = await _crudService.GetAsync();

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(products);

        await writer.FlushAsync();
        return memoryStream.ToArray();
    }
}
