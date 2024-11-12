﻿namespace ProductManager.Application.Feature.Orders.Command;

public class AddOrUpdateOrderCommand : ICommand<ApiResponse>
{
    public AddOrUpdateOrderCommand(Order supplier)
    {
        Order = supplier;
    }
    public Order Order { get; set; }
}
internal class AddOrUpdateOrderCommandHandler : ICommandHandler<AddOrUpdateOrderCommand, ApiResponse>
{
    private readonly ICrudService<Order> _supplierService;
    private readonly IUnitOfWork _unitOfWork;
    public AddOrUpdateOrderCommandHandler(ICrudService<Order> supplierService, IUnitOfWork unitOfWork)
    {
        _supplierService = supplierService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateOrderCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            if (command.Order.Id == null!)
            {
                command.Order.Id = UlidExtension.Generate();
                await _supplierService.AddAsync(command.Order, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, "Order created successfully");
            }
            await _supplierService.UpdateAsync(command.Order, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, "Order updated successfully");
        }
    }
}
