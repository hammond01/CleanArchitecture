using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Category.Commands;
using ProductManager.Application.Feature.Category.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.CategoryDto;
namespace ProductManager.Api.Controllers;

public class CategoryController : ConBase
{
    private readonly Dispatcher _dispatcher;

    public CategoryController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<ApiResponse> GetCategories()
    {
        var data = await _dispatcher.DispatchAsync(new GetCategories());
        data.Result = ((List<Categories>)data.Result).Adapt<List<GetCategoryDto>>();
        return data;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetCategory(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
        data.Result = ((Categories)data.Result).Adapt<GetCategoryDto>();
        return data;
    }

    [HttpPost]
    public async Task<ApiResponse> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        var data = createCategoryDto.Adapt<Categories>();
        return await _dispatcher.DispatchAsync(new AddOrUpdateCategoryCommand(data));
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateCategory(string id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
        var category = (Categories)apiResponse.Result;
        updateCategoryDto.Adapt(category);
        return await _dispatcher.DispatchAsync(new AddOrUpdateCategoryCommand(category));
    }
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteCategory(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
        var category = (Categories)apiResponse.Result;
        return await _dispatcher.DispatchAsync(new DeleteCategoryCommand(category));
    }
}
