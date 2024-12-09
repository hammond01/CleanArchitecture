namespace ProductManager.Application.Feature.Supplier.Commands;

public class DeleteSupplierCommand : ICommand<ApiResponse>
{
    public DeleteSupplierCommand(Suppliers suppliers)
    {
        Suppliers = suppliers;
    }

    public Suppliers Suppliers { get; set; }
}
public class DeleteSupplierCommandHandler : ICommandHandler<DeleteSupplierCommand, ApiResponse>
{
    private readonly ICrudService<Suppliers> _supplierService;

    public DeleteSupplierCommandHandler(ICrudService<Suppliers> supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        await _supplierService.DeleteAsync(request.Suppliers, cancellationToken);

        return new ApiResponse
        {
            Message = CRUDMessage.DeleteSuccess
        };
    }
}
