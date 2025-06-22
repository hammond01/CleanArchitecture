using System.Data;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Persistence.Extensions;
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
                return new ApiResponse(201, CRUDMessage.CreateSuccess);
            }
            await _supplierService.UpdateAsync(command.Supplier, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.UpdateSuccess);
        }
    }
}
