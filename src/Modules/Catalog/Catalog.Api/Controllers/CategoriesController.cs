using BuildingBlocks.Api.Controllers;
using Catalog.Application.DTOs;
using Catalog.Application.Features.Categories.Commands;
using Catalog.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

/// <summary>
/// Category API Controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : BaseController
{
    private readonly GetCategoriesQueryHandler _getCategoriesHandler;
    private readonly GetCategoryByIdQueryHandler _getCategoryByIdHandler;
    private readonly CreateOrUpdateCategoryCommandHandler _createOrUpdateHandler;
    private readonly DeleteCategoryCommandHandler _deleteHandler;

    public CategoriesController(
        GetCategoriesQueryHandler getCategoriesHandler,
        GetCategoryByIdQueryHandler getCategoryByIdHandler,
        CreateOrUpdateCategoryCommandHandler createOrUpdateHandler,
        DeleteCategoryCommandHandler deleteHandler)
    {
        _getCategoriesHandler = getCategoriesHandler;
        _getCategoryByIdHandler = getCategoryByIdHandler;
        _createOrUpdateHandler = createOrUpdateHandler;
        _deleteHandler = deleteHandler;
    }

    /// <summary>
    /// Get all categories with optional search filtering
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategories(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetCategoriesQuery
        {
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var categories = await _getCategoriesHandler.HandleAsync(query);
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(string id)
    {
        var query = new GetCategoryByIdQuery(id);
        var category = await _getCategoryByIdHandler.HandleAsync(query);

        if (category == null)
        {
            return NotFound($"Category with ID {id} not found");
        }

        return Ok(category);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateOrUpdateCategoryCommand command)
    {
        var categoryId = await _createOrUpdateHandler.HandleAsync(command);
        return Created($"/api/v1/categories/{categoryId}", new { id = categoryId });
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(string id, [FromBody] CreateOrUpdateCategoryCommand command)
    {
        var updateCommand = command with { Id = id };
        var categoryId = await _createOrUpdateHandler.HandleAsync(updateCommand);
        return Ok(new { id = categoryId });
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await _deleteHandler.HandleAsync(command);

        if (!result)
        {
            return NotFound($"Category with ID {id} not found");
        }

        return Ok(new { message = "Category deleted successfully" });
    }
}
