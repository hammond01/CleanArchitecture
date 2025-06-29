using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Category.Commands;
using ProductManager.Application.Feature.Category.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.CategoryDto;
namespace ProductManager.Api.Controllers;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
public class CategoryController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public CategoryController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    [HttpGet]
    [LogAction("Get all categories")]
    public async Task<ActionResult<ApiResponse>> GetCategories()
    {
        var data = await _dispatcher.DispatchAsync(new GetCategories());
        data.Result = data.Result.Adapt<List<GetCategoryDto>>();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [LogAction("Get category by ID")]
    public async Task<ActionResult<ApiResponse>> GetCategory(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
        data.Result = data.Result.Adapt<GetCategoryDto>();
        return Ok(data);
    }

    [HttpPost]
    [LogAction("Create new category")]
    public async Task<ActionResult<ApiResponse>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        var data = createCategoryDto.Adapt<Categories>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateCategoryCommand(data));
        var createdCategory = (Categories)result.Result;
        return Created($"/api/v1.0/categories/{createdCategory.Id}", result);
    }

    [HttpPut("{id}")]
    [LogAction("Update category")]
    public async Task<ActionResult<ApiResponse>> UpdateCategory(string id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
        if (apiResponse.Result is not Categories category)
        {
            return NotFound(new ApiResponse(404, "Category not found"));
        }
        updateCategoryDto.Adapt(category);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateCategoryCommand(category));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [LogAction("Delete category")]
    public async Task<ActionResult<ApiResponse>> DeleteCategory(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
        if (apiResponse.Result is not Categories category)
        {
            return NotFound(new ApiResponse(404, "Category not found"));
        }
        await _dispatcher.DispatchAsync(new DeleteCategoryCommand(category));
        return NoContent();
    }
}
