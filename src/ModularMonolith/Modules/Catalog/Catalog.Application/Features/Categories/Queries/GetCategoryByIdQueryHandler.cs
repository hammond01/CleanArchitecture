using BuildingBlocks.Application.CQRS;
using Catalog.Application.DTOs;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Queries;

/// <summary>
/// Handler for GetCategoryByIdQuery
/// </summary>
public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository
            .GetQueryableSet()
            .Where(c => c.Id == query.Id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
                PictureLink = c.PictureLink,
                CreatedDateTime = c.CreatedDateTime,
                UpdatedDateTime = c.UpdatedDateTime
            })
            .FirstOrDefaultAsync(cancellationToken);

        return category;
    }
}
