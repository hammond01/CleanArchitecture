namespace ProductManager.Application.Feature.Supplier.Commands;

public class AddOrUpdateSupplierCommand : ICommand<ApiResponse>
{
    public AddOrUpdateSupplierCommand(Suppliers supplier)
    {
        Supplier = supplier;
    }
    public Suppliers Supplier { get; set; }
}
internal class AddOrUpdateSupplierCommandHandler : ICommandHandler<AddOrUpdateSupplierCommand, ApiResponse>
{
    private readonly ICrudService<Suppliers> _supplierService;
    private readonly IUnitOfWork _unitOfWork;
    public AddOrUpdateSupplierCommandHandler(ICrudService<Suppliers> supplierService, IUnitOfWork unitOfWork)
    {
        _supplierService = supplierService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            if (command.Supplier.Id == null!)
            {
                command.Supplier.Id = UlidExtension.Generate();
                await _supplierService.AddAsync(command.Supplier, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, "Supplier created successfully");
            }
            await _supplierService.UpdateAsync(command.Supplier, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, "Supplier updated successfully");
        }
    }
}
