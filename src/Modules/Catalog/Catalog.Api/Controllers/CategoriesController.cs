using BuildingBlocks.Api.Controllers;
using BuildingBlocks.Application.Dispatcher;
using Catalog.Application.DTOs;
using Catalog.Application.Features.Categories.Commands;
using Catalog.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

/// <summary>
/// Category API Controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : BaseController
{
    private readonly IDispatcher _dispatcher;

    public CategoriesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
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

        var categories = await _dispatcher.DispatchAsync(query);
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(string id)
    {
        var query = new GetCategoryByIdQuery(id);
        var category = await _dispatcher.DispatchAsync(query);

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
    [Authorize]
    public async Task<IActionResult> CreateCategory([FromBody] CreateOrUpdateCategoryCommand command)
    {
        var categoryId = await _dispatcher.DispatchAsync(command);
        return Created($"/api/v1/categories/{categoryId}", new { id = categoryId });
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategory(string id, [FromBody] CreateOrUpdateCategoryCommand command)
    {
        var updateCommand = command with { Id = id };
        var categoryId = await _dispatcher.DispatchAsync(updateCommand);
        return Ok(new { id = categoryId });
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await _dispatcher.DispatchAsync(command);

        if (!result)
        {
            return NotFound($"Category with ID {id} not found");
        }

        return Ok(new { message = "Category deleted successfully" });
    }
}
