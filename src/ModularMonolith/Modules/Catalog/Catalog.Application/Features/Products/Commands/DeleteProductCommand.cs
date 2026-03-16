using BuildingBlocks.Application.CQRS;

namespace Catalog.Application.Features.Products.Commands;

/// <summary>
/// Command to delete a product
/// </summary>
public record DeleteProductCommand(string ProductId) : ICommand<bool>;
