using BuildingBlocks.Application.CQRS;
using Catalog.Application.DTOs;

namespace Catalog.Application.Features.Categories.Queries;

/// <summary>
/// Query to get all categories with optional filtering
/// </summary>
public record GetCategoriesQuery : IQuery<List<CategoryDto>>
{
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
