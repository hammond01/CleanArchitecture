using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Category.Queries;
using ProductManager.Domain.Common;
namespace ProductManager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly Dispatcher _dispatcher;
    public CategoryController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    [HttpGet]
    [Authorize]
    public async Task<ApiResponse> GetCategories() => await _dispatcher.DispatchAsync(new GetCategories());

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetCategory(string id) => await _dispatcher.DispatchAsync(new GetCategoryById(id));
}
