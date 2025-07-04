using System.Data;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Persistence.Extensions;

namespace ProductManager.Application.Feature.OrderDetail.Commands;

public class AddOrUpdateOrderDetailCommand : ICommand<ApiResponse>
{
    public AddOrUpdateOrderDetailCommand(OrderDetails orderDetail)
    {
        OrderDetail = orderDetail;
    }
    public OrderDetails OrderDetail { get; set; }
}

internal class AddOrUpdateOrderDetailHandler : ICommandHandler<AddOrUpdateOrderDetailCommand, ApiResponse>
{
    private readonly ICrudService<OrderDetails> _orderDetailService;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrUpdateOrderDetailHandler(ICrudService<OrderDetails> orderDetailService, IUnitOfWork unitOfWork)
    {
        _orderDetailService = orderDetailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateOrderDetailCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            if (command.OrderDetail.Id == null!)
            {
                command.OrderDetail.Id = UlidExtension.Generate();
                await _orderDetailService.AddAsync(command.OrderDetail, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, CRUDMessage.CreateSuccess, command.OrderDetail);
            }
            await _orderDetailService.UpdateAsync(command.OrderDetail, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.UpdateSuccess);
        }
    }
}
