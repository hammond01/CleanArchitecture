using Shared.Common.Mediator;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Categories.Commands;
using ProductCatalog.Application.Categories.Queries;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.API.Controllers;

/// <summary>
/// Categories controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
    {
        var query = new GetAllCategoriesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategoryById(string id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound($"Category with ID {id} not found");

        return Ok(result);
    }

    /// <summary>
    /// Create new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<string>> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCategoryById), new { id = result }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategory(string id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.CategoryId)
            return BadRequest("Category ID mismatch");

        try
        {
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete category
    /// </summary>
    [HttpDelete("{id}")]
    public ActionResult DeleteCategory(string id)
    {
        // Implementation for delete would go here
        // For now, return NotImplemented
        return StatusCode(501, "Delete operation not implemented yet");
    }
}
