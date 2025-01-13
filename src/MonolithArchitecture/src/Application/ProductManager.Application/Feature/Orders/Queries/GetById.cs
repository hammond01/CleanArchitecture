namespace ProductManager.Application.Feature.Orders.Queries;

public record GetOrderByIdQuery : IQuery<ApiResponse>
{
    public GetOrderByIdQuery(string categoryId)
    {
        OrderId = categoryId;
    }
    public string OrderId { get; set; }
}
public class GetOrderByIdHandler : IQueryHandler<GetOrderByIdQuery, ApiResponse>
{
    private readonly ICrudService<Order> _crudService;
    public GetOrderByIdHandler(ICrudService<Order> crudService)
    {
        _crudService = crudService;
    }

    public async Task<ApiResponse> HandleAsync(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var categories = await _crudService.GetByIdAsync(request.OrderId);
        return categories == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", categories);
    }
}
