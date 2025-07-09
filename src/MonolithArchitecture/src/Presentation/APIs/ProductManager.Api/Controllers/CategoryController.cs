using System.ComponentModel.DataAnnotations;
using System.Net;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Category.Commands;
using ProductManager.Application.Feature.Category.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Infrastructure.Middleware;
using ProductManager.Shared.DTOs.CategoryDto;

namespace ProductManager.Api.Controllers;

/// <summary>
/// API Controller for managing product categories with hierarchical support
/// </summary>
/// <remarks>
/// This controller provides comprehensive CRUD operations for category management including:
/// • Hierarchical category structure support
/// • Enhanced validation and error handling
/// • Entity locking for concurrent modification prevention
/// • Performance optimization with caching headers
/// • Comprehensive audit logging and monitoring
/// </remarks>
[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
[ApiExplorerSettings(GroupName = "Categories")]
public class CategoryController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    /// <summary>
    /// Initializes a new instance of the CategoryController
    /// </summary>
    /// <param name="dispatcher">The command/query dispatcher for CQRS pattern implementation</param>
    public CategoryController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    /// <summary>
    /// Retrieves all categories with hierarchical structure
    /// </summary>
    /// <returns>A list of all categories with their hierarchical relationships</returns>
    /// <response code="200">Successfully retrieved all categories</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet]
    [LogAction("Get all categories")]
    [ProducesResponseType(typeof(ApiResponse<List<GetCategoryDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ApiResponse>> GetCategories()
    {
        try
        {
            // Add performance tracking
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var data = await _dispatcher.DispatchAsync(new GetCategories());
            data.Result = data.Result.Adapt<List<GetCategoryDto>>();

            stopwatch.Stop();

            // Add performance headers
            Response.Headers["X-Response-Time"] = $"{stopwatch.ElapsedMilliseconds}ms";
            Response.Headers["X-Total-Count"] = ((List<GetCategoryDto>)data.Result).Count.ToString();

            // Add caching headers for better performance
            Response.Headers["Cache-Control"] = "public, max-age=300"; // 5 minutes cache
            Response.Headers["ETag"] = $"\"{DateTime.UtcNow.Ticks}\"";

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                new ApiResponse(500, $"Error retrieving categories: {ex.Message}"));
        }
    }

    /// <summary>
    /// Retrieves a specific category by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the category</param>
    /// <returns>The category details if found</returns>
    /// <response code="200">Successfully retrieved the category</response>
    /// <response code="400">Invalid category ID provided</response>
    /// <response code="404">Category not found</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("{id}")]
    [LogAction("Get category by ID")]
    [ProducesResponseType(typeof(ApiResponse<GetCategoryDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ApiResponse>> GetCategory(
        [Required][StringLength(50, MinimumLength = 1)] string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new ApiResponse(400, "Category ID is required and cannot be empty"));
        }

        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var data = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));

            if (data.Result == null)
            {
                return NotFound(new ApiResponse(404, $"Category with ID '{id}' was not found"));
            }

            data.Result = data.Result.Adapt<GetCategoryDto>();
            stopwatch.Stop();

            // Add performance and caching headers
            Response.Headers["X-Response-Time"] = $"{stopwatch.ElapsedMilliseconds}ms";
            Response.Headers["Cache-Control"] = "public, max-age=600"; // 10 minutes cache for individual items
            Response.Headers["ETag"] = $"\"{data.Result.GetHashCode()}\"";

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                new ApiResponse(500, $"Error retrieving category: {ex.Message}"));
        }
    }

    /// <summary>
    /// Creates a new category with hierarchical support
    /// </summary>
    /// <param name="createCategoryDto">The category data to create</param>
    /// <returns>The created category details</returns>
    /// <response code="201">Category successfully created</response>
    /// <response code="400">Invalid category data provided</response>
    /// <response code="409">Category with same name already exists</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpPost]
    [LogAction("Create new category")]
    [ProducesResponseType(typeof(ApiResponse<Categories>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ApiResponse>> CreateCategory(
        [FromBody][Required] CreateCategoryDto createCategoryDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ApiResponse(400, $"Validation failed: {string.Join(", ", errors)}"));
        }

        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var category = createCategoryDto.Adapt<Categories>();
            var result = await _dispatcher.DispatchAsync(new AddOrUpdateCategoryCommand(category));

            stopwatch.Stop();

            if (result.StatusCode == 201)
            {
                var createdCategory = (Categories)result.Result;

                // Add performance headers
                Response.Headers["X-Response-Time"] = $"{stopwatch.ElapsedMilliseconds}ms";
                Response.Headers["X-Resource-Created"] = createdCategory.Id;

                // Return created response with location header
                var location = Url.Action(nameof(GetCategory), new { id = createdCategory.Id });
                return Created(location, result);
            }

            return StatusCode(result.StatusCode, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse(400, $"Invalid category data: {ex.Message}"));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse(409, $"Category creation conflict: {ex.Message}"));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                new ApiResponse(500, $"Error creating category: {ex.Message}"));
        }
    }

    /// <summary>
    /// Updates an existing category with entity locking for concurrent modification prevention
    /// </summary>
    /// <param name="id">The unique identifier of the category to update</param>
    /// <param name="updateCategoryDto">The updated category data</param>
    /// <returns>The updated category details</returns>
    /// <response code="200">Category successfully updated</response>
    /// <response code="400">Invalid category data or ID provided</response>
    /// <response code="404">Category not found</response>
    /// <response code="409">Category is locked by another user</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpPut("{id}")]
    [LogAction("Update category")]
    [EntityLock("Category", "id", lockTimeoutMinutes: 5)]
    [ProducesResponseType(typeof(ApiResponse<Categories>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ApiResponse>> UpdateCategory(
        [Required][StringLength(50, MinimumLength = 1)] string id,
        [FromBody][Required] UpdateCategoryDto updateCategoryDto)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new ApiResponse(400, "Category ID is required and cannot be empty"));
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ApiResponse(400, $"Validation failed: {string.Join(", ", errors)}"));
        }

        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // First, get the existing category
            var apiResponse = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
            if (apiResponse.Result is not Categories category)
            {
                return NotFound(new ApiResponse(404, $"Category with ID '{id}' was not found"));
            }

            // Apply updates
            updateCategoryDto.Adapt(category);

            // Update the category
            var result = await _dispatcher.DispatchAsync(new AddOrUpdateCategoryCommand(category));

            stopwatch.Stop();

            // Add performance headers
            Response.Headers["X-Response-Time"] = $"{stopwatch.ElapsedMilliseconds}ms";
            Response.Headers["X-Resource-Updated"] = id;
            Response.Headers["Last-Modified"] = DateTime.UtcNow.ToString("R");

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse(400, $"Invalid category data: {ex.Message}"));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse(409, $"Category update conflict: {ex.Message}"));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                new ApiResponse(500, $"Error updating category: {ex.Message}"));
        }
    }

    /// <summary>
    /// Deletes a category with entity locking and cascade validation
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">Category successfully deleted</response>
    /// <response code="400">Invalid category ID provided</response>
    /// <response code="404">Category not found</response>
    /// <response code="409">Category is locked by another user or has dependent entities</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpDelete("{id}")]
    [LogAction("Delete category")]
    [EntityLock("Category", "id", lockTimeoutMinutes: 5)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ApiResponse>> DeleteCategory(
        [Required][StringLength(50, MinimumLength = 1)] string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new ApiResponse(400, "Category ID is required and cannot be empty"));
        }

        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // First, get the existing category
            var apiResponse = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
            if (apiResponse.Result is not Categories category)
            {
                return NotFound(new ApiResponse(404, $"Category with ID '{id}' was not found"));
            }

            // Delete the category
            await _dispatcher.DispatchAsync(new DeleteCategoryCommand(category));

            stopwatch.Stop();

            // Add performance headers
            Response.Headers["X-Response-Time"] = $"{stopwatch.ElapsedMilliseconds}ms";
            Response.Headers["X-Resource-Deleted"] = id;
            Response.Headers["X-Deletion-Timestamp"] = DateTime.UtcNow.ToString("O");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse(409, $"Cannot delete category: {ex.Message}"));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                new ApiResponse(500, $"Error deleting category: {ex.Message}"));
        }
    }
}
