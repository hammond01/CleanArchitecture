namespace ProductManager.Application.Feature.Orders.Queries;

public record GetOrders : IQuery<ApiResponse>;
internal class GetsOrderHandler : IQueryHandler<GetOrders, ApiResponse>
{
    private readonly ICrudService<Order> _crudService;
    public GetsOrderHandler(ICrudService<Order> crudService)
    {
        _crudService = crudService;
    }
    public async Task<ApiResponse> HandleAsync(GetOrders query, CancellationToken cancellationToken = default)
    {
        var response = await _crudService.GetAsync(cancellationToken);
        return new ApiResponse(200, "Get suppliers successfully", response);
    }
}
