namespace ProductManager.Api.Controllers;

public class OrderController : ConBase
{
    private readonly Dispatcher _dispatcher;
    private readonly IMapper _mapper;
    public OrderController(Dispatcher dispatcher, IMapper mapper)
    {
        _dispatcher = dispatcher;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<ApiResponse> GetOrders()
    {
        var data = await _dispatcher.DispatchAsync(new GetOrders());
        data.Result = _mapper.Map<List<GetOrderDto>>(data.Result);
        return data;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetOrder(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id));
        data.Result = _mapper.Map<GetOrderDto>(data.Result);
        return data;
    }

    [HttpPost]
    public async Task<ApiResponse> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        var data = _mapper.Map<Order>(createOrderDto);
        return await _dispatcher.DispatchAsync(new AddOrUpdateOrderCommand(data));
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateOrder(string id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id));
        var category = _mapper.Map<Order>(apiResponse.Result);
        _mapper.Map(updateOrderDto, category);
        return await _dispatcher.DispatchAsync(new AddOrUpdateOrderCommand(category));
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteOrder(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id));
        var category = _mapper.Map<Order>(apiResponse.Result);
        if (category == null)
        {
            return new ApiResponse
            {
                StatusCode = 404, Message = "Order not found"
            };
        }
        return await _dispatcher.DispatchAsync(new DeleteOrderCommand(category));
    }
}
