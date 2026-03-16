using BuildingBlocks.Application.CQRS;
using Catalog.Application.DTOs;

namespace Catalog.Application.Features.Categories.Queries;

/// <summary>
/// Query to get a category by ID
/// </summary>
public record GetCategoryByIdQuery(string Id) : IQuery<CategoryDto?>;
