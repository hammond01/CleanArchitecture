using BuildingBlocks.Application.CQRS;
using Catalog.Application.DTOs;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Queries;

/// <summary>
/// Handler for GetCategoriesQuery
/// </summary>
public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryDto>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var categoriesQuery = _categoryRepository.GetQueryableSet();

        // Apply search filter
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            categoriesQuery = categoriesQuery.Where(c =>
                c.CategoryName.Contains(query.SearchTerm) ||
                (c.Description != null && c.Description.Contains(query.SearchTerm)));
        }

        // Apply pagination
        var categories = await categoriesQuery
            .OrderBy(c => c.CategoryName)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
                PictureLink = c.PictureLink,
                CreatedDateTime = c.CreatedDateTime,
                UpdatedDateTime = c.UpdatedDateTime
            })
            .ToListAsync(cancellationToken);

        return categories;
    }
}
