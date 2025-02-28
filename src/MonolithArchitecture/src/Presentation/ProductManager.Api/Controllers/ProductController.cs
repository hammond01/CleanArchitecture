namespace ProductManager.Api.Controllers;

public class ProductController : ConBase
{
    private readonly Dispatcher _dispatcher;
    private readonly IMapper _mapper;
    public ProductController(Dispatcher dispatcher, IMapper mapper)
    {
        _dispatcher = dispatcher;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<ApiResponse> GetProducts()
    {
        var data = await _dispatcher.DispatchAsync(new GetProducts());
        data.Result = _mapper.Map<List<GetProductDto>>(data.Result);
        return data;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetProduct(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        data.Result = _mapper.Map<GetProductDto>(data.Result);
        return data;
    }

    [HttpPost]
    public async Task<ApiResponse> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var data = _mapper.Map<Products>(createProductDto);
        return await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(data));
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateProduct(string id, [FromBody] UpdateProductDto updateProductDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        var category = _mapper.Map<Products>(apiResponse.Result);
        _mapper.Map(updateProductDto, category);
        return await _dispatcher.DispatchAsync(new AddOrUpdateProductCommand(category));
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteProduct(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetProductByIdQuery(id));
        var category = _mapper.Map<Products>(apiResponse.Result);
        if (category == null)
        {
            return new ApiResponse
            {
                StatusCode = 404, Message = "Product not found"
            };
        }
        return await _dispatcher.DispatchAsync(new DeleteProductCommand(category));
    }
}
