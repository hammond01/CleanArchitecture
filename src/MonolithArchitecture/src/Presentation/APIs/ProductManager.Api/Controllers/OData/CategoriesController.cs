using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ProductManager.Domain.Entities;
using ProductManager.Persistence;

namespace ProductManager.Api.Controllers.OData;

/// <summary>
/// OData controller for Categories with advanced query capabilities
/// </summary>
public class CategoriesController : ODataController
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Get categories with OData query support
    /// </summary>
    /// <returns>Queryable collection of categories</returns>
    /// <response code="200">Returns the categories</response>
    /// <response code="400">If the query is invalid</response>
    [HttpGet]
    [EnableQuery(PageSize = 50, MaxTop = 1000)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IQueryable<Categories> Get()
    {
        return _context.Categories;
    }

    /// <summary>
    /// Get a specific category by ID with OData query support
    /// </summary>
    /// <param name="key">Category ID</param>
    /// <returns>Category entity</returns>
    /// <response code="200">Returns the category</response>
    /// <response code="404">If the category is not found</response>
    [HttpGet]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute] string key)
    {
        var category = _context.Categories.Find(key);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="category">Category to create</param>
    /// <returns>Created category</returns>
    /// <response code="201">Returns the created category</response>
    /// <response code="400">If the category is invalid</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] Categories category)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        category.Id = Guid.NewGuid().ToString();
        category.CreatedDateTime = DateTimeOffset.UtcNow;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { key = category.Id }, category);
    }

    /// <summary>
    /// Update a category
    /// </summary>
    /// <param name="key">Category ID</param>
    /// <param name="category">Updated category data</param>
    /// <returns>Updated category</returns>
    /// <response code="200">Returns the updated category</response>
    /// <response code="400">If the category is invalid</response>
    /// <response code="404">If the category is not found</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromRoute] string key, [FromBody] Categories category)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingCategory = await _context.Categories.FindAsync(key);
        if (existingCategory == null)
        {
            return NotFound();
        }

        existingCategory.CategoryName = category.CategoryName;
        existingCategory.Description = category.Description;
        existingCategory.Picture = category.Picture;
        existingCategory.PictureLink = category.PictureLink;
        existingCategory.UpdatedDateTime = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(existingCategory);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    /// <param name="key">Category ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Category deleted successfully</response>
    /// <response code="404">If the category is not found</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        var category = await _context.Categories.FindAsync(key);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
