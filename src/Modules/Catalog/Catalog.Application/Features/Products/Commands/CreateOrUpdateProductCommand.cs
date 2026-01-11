using BuildingBlocks.Application.CQRS;

namespace Catalog.Application.Features.Products.Commands;

/// <summary>
/// Command to create or update a product
/// </summary>
public record CreateOrUpdateProductCommand : ICommand<string>
{
    public string? Id { get; init; }
    public string ProductName { get; init; } = null!;
    public string CategoryId { get; init; } = null!;
    public string? QuantityPerUnit { get; init; }
    public decimal? UnitPrice { get; init; }
    public short? UnitsInStock { get; init; }
    public short? UnitsOnOrder { get; init; }
    public short? ReorderLevel { get; init; }
    public bool Discontinued { get; init; }
}
