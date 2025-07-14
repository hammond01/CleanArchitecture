using Shared.Common.Mediator;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Features.Products.Commands;

/// <summary>
/// Create product command
/// </summary>
public record CreateProductCommand : IRequest<string>
{
    public string ProductName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string CategoryId { get; init; } = string.Empty;
    public string SupplierId { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int UnitsInStock { get; init; }
    public int ReorderLevel { get; init; } = 10;
    public string? ImageUrl { get; init; }
    public decimal Weight { get; init; }
    public string? Dimensions { get; init; }
}

/// <summary>
/// Update product command
/// </summary>
public record UpdateProductCommand : IRequest<bool>
{
    public string Id { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal UnitPrice { get; init; }
    public int ReorderLevel { get; init; }
    public string? ImageUrl { get; init; }
    public decimal Weight { get; init; }
    public string? Dimensions { get; init; }
}

/// <summary>
/// Update product stock command
/// </summary>
public record UpdateProductStockCommand : IRequest<bool>
{
    public string Id { get; init; } = string.Empty;
    public int NewStock { get; init; }
}

/// <summary>
/// Reduce product stock command
/// </summary>
public record ReduceProductStockCommand : IRequest<bool>
{
    public string Id { get; init; } = string.Empty;
    public int Quantity { get; init; }
}

/// <summary>
/// Increase product stock command
/// </summary>
public record IncreaseProductStockCommand : IRequest<bool>
{
    public string Id { get; init; } = string.Empty;
    public int Quantity { get; init; }
}

/// <summary>
/// Discontinue product command
/// </summary>
public record DiscontinueProductCommand : IRequest<bool>
{
    public string Id { get; init; } = string.Empty;
}

/// <summary>
/// Activate product command
/// </summary>
public record ActivateProductCommand : IRequest<bool>
{
    public string Id { get; init; } = string.Empty;
}

/// <summary>
/// Delete product command
/// </summary>
public record DeleteProductCommand : IRequest<bool>
{
    public string Id { get; init; } = string.Empty;
}
