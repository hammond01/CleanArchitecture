using AutoMapper;
using ProductManager.Domain.Entities;
namespace ProductManager.Api.Controllers;

public class CategoryController : ConBase
{
    private readonly Dispatcher _dispatcher;
    private readonly IMapper _mapper;
    public CategoryController(Dispatcher dispatcher, IMapper mapper)
    {
        _dispatcher = dispatcher;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<ApiResponse> GetCategories()
        => await _dispatcher.DispatchAsync(new GetCategories());

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetCategory(string id)
        => await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));

    [HttpPost]
    public async Task<ApiResponse> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        var data = _mapper.Map<Categories>(createCategoryDto);
        return await _dispatcher.DispatchAsync(new AddOrUpdateCategoryCommand(data));
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateCategory(string id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
        var category = _mapper.Map<Categories>(apiResponse.Result);
        _mapper.Map(updateCategoryDto, category);
        return await _dispatcher.DispatchAsync(new AddOrUpdateCategoryCommand(category));
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteCategory(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetCategoryByIdQuery(id));
        var category = _mapper.Map<Categories>(apiResponse.Result);
        if (category == null)
        {
            return new ApiResponse
            {
                StatusCode = 404, Message = "Category not found"
            };
        }
        return await _dispatcher.DispatchAsync(new DeleteCategoryCommand(category));
    }
}
