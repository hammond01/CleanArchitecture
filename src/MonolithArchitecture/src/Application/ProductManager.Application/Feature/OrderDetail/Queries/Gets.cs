using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.OrderDetail.Queries;

public record GetOrderDetails : IQuery<ApiResponse>;

internal class GetOrderDetailsHandler : IQueryHandler<GetOrderDetails, ApiResponse>
{
    private readonly ICrudService<OrderDetails> _orderDetailService;

    public GetOrderDetailsHandler(ICrudService<OrderDetails> orderDetailService)
    {
        _orderDetailService = orderDetailService;
    }

    public async Task<ApiResponse> HandleAsync(GetOrderDetails query, CancellationToken cancellationToken = default)
    {
        var response = await _orderDetailService.GetAsync();
        return new ApiResponse(200, "Get order details successfully", response);
    }
}
