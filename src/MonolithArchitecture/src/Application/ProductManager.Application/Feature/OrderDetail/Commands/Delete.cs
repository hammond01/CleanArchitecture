using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.OrderDetail.Commands;

public class DeleteOrderDetailCommand : ICommand<ApiResponse>
{
    public DeleteOrderDetailCommand(OrderDetails orderDetail)
    {
        OrderDetail = orderDetail;
    }
    public OrderDetails OrderDetail { get; set; }
}

public class DeleteOrderDetailCommandHandler : ICommandHandler<DeleteOrderDetailCommand, ApiResponse>
{
    private readonly ICrudService<OrderDetails> _orderDetailService;

    public DeleteOrderDetailCommandHandler(ICrudService<OrderDetails> orderDetailService)
    {
        _orderDetailService = orderDetailService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteOrderDetailCommand request, CancellationToken cancellationToken)
    {
        await _orderDetailService.DeleteAsync(request.OrderDetail, cancellationToken);

        return new ApiResponse
        {
            StatusCode = 204,
            Message = CRUDMessage.DeleteSuccess
        };
    }
}
