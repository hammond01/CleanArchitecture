using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.OrderDetail.Queries;

public record GetOrderDetailByIdQuery : IQuery<ApiResponse>
{
    public GetOrderDetailByIdQuery(string orderDetailId)
    {
        OrderDetailId = orderDetailId;
    }
    public string OrderDetailId { get; set; }
}

public class GetOrderDetailByIdHandler : IQueryHandler<GetOrderDetailByIdQuery, ApiResponse>
{
    private readonly ICrudService<OrderDetails> _orderDetailService;

    public GetOrderDetailByIdHandler(ICrudService<OrderDetails> orderDetailService)
    {
        _orderDetailService = orderDetailService;
    }

    public async Task<ApiResponse> HandleAsync(GetOrderDetailByIdQuery request, CancellationToken cancellationToken)
    {
        var orderDetails = await _orderDetailService.GetByIdAsync(request.OrderDetailId);
        return orderDetails == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", orderDetails);
    }
}
