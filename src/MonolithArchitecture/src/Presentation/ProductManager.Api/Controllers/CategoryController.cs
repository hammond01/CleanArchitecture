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
        => await _dispatcher.DispatchAsync(new GetCategories());

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetCategory(string id)
        => await _dispatcher.DispatchAsync(new GetCategoryById(id));

    [HttpPost]
    public async Task<ApiResponse> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        => await _dispatcher.DispatchAsync(new CreateCategoryCommand(createCategoryDto));
}
