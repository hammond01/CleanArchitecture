using BuildingBlocks.Application.CQRS;

namespace Catalog.Application.Features.Categories.Commands;

/// <summary>
/// Command to create or update a category
/// </summary>
public record CreateOrUpdateCategoryCommand : ICommand<string>
{
    public string? Id { get; init; }
    public string CategoryName { get; init; } = null!;
    public string? Description { get; init; }
    public string? PictureLink { get; init; }
}
