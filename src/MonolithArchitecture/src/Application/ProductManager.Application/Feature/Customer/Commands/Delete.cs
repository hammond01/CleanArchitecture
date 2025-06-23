using System.Data;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
namespace ProductManager.Application.Feature.Customer.Commands;

public class DeleteCustomerCommand : ICommand<ApiResponse>
{
    public DeleteCustomerCommand(Customers customer)
    {
        Customer = customer;
    }
    public Customers Customer { get; set; }
}
internal class DeleteCustomerHandler : ICommandHandler<DeleteCustomerCommand, ApiResponse>
{
    private readonly ICrudService<Customers> _customerService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerHandler(ICrudService<Customers> customerService, IUnitOfWork unitOfWork)
    {
        _customerService = customerService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> HandleAsync(DeleteCustomerCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            await _customerService.DeleteAsync(command.Customer, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.DeleteSuccess);
        }
    }
}
