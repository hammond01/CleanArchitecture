namespace ProductManager.Api.Controllers;

public class SupplierController : ConBase
{
    private readonly Dispatcher _dispatcher;
    private readonly IMapper _mapper;
    public SupplierController(Dispatcher dispatcher, IMapper mapper)
    {
        _dispatcher = dispatcher;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<ApiResponse> GetSuppliers()
    {
        var data = await _dispatcher.DispatchAsync(new GetSuppliers());
        data.Result = _mapper.Map<List<GetSupplierDto>>(data.Result);
        return data;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetSupplier(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetSupplierByIdQuery(id));
        data.Result = _mapper.Map<GetSupplierDto>(data.Result);
        return data;
    }

    [HttpPost]
    public async Task<ApiResponse> CreateSupplier([FromBody] CreateSupplierDto createSupplierDto)
    {
        var data = _mapper.Map<Suppliers>(createSupplierDto);
        return await _dispatcher.DispatchAsync(new AddOrUpdateSupplierCommand(data));
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateSupplier(string id, [FromBody] UpdateSupplierDto updateSupplierDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetSupplierByIdQuery(id));
        var category = _mapper.Map<Suppliers>(apiResponse.Result);
        _mapper.Map(updateSupplierDto, category);
        return await _dispatcher.DispatchAsync(new AddOrUpdateSupplierCommand(category));
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteSupplier(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetSupplierByIdQuery(id));
        if (apiResponse.IsSuccessStatusCode == false)
        {
            return apiResponse;
        }
        var category = _mapper.Map<Suppliers>(apiResponse.Result);
        return await _dispatcher.DispatchAsync(new DeleteSupplierCommand(category));
    }
}
