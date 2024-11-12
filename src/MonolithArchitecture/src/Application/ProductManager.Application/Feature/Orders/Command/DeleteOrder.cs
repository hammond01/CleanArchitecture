namespace ProductManager.Application.Feature.Orders.Command;

public class DeleteOrderCommand : ICommand<ApiResponse>
{
    public DeleteOrderCommand(Order suppliers)
    {
        Orders = suppliers;
    }

    public Order Orders { get; set; }
}
public class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand, ApiResponse>
{
    private readonly ICrudService<Order> _supplierService;

    public DeleteOrderCommandHandler(ICrudService<Order> supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        await _supplierService.DeleteAsync(request.Orders, cancellationToken);

        return new ApiResponse
        {
            Message = "Order deleted successfully"
        };
    }
}
