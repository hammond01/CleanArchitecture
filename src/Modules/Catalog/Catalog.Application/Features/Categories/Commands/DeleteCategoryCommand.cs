using BuildingBlocks.Application.CQRS;

namespace Catalog.Application.Features.Categories.Commands;

/// <summary>
/// Command to delete a category
/// </summary>
public record DeleteCategoryCommand(string Id) : ICommand<bool>;
