using System.Data;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
namespace ProductManager.Application.Feature.Customer.Commands;

public class AddOrUpdateCustomerCommand : ICommand<ApiResponse>
{
    public AddOrUpdateCustomerCommand(Customers customer)
    {
        Customer = customer;
    }
    public Customers Customer { get; set; }
}
internal class AddOrUpdateCustomerHandler : ICommandHandler<AddOrUpdateCustomerCommand, ApiResponse>
{
    private readonly ICrudService<Customers> _customerService;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrUpdateCustomerHandler(ICrudService<Customers> customerService, IUnitOfWork unitOfWork)
    {
        _customerService = customerService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            // Check if customer exists
            var existingCustomer = await _customerService.GetByIdAsync(command.Customer.Id);

            if (existingCustomer == null)
            {
                await _customerService.AddAsync(command.Customer, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, CRUDMessage.CreateSuccess);
            }

            await _customerService.UpdateAsync(command.Customer, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.UpdateSuccess);
        }
    }
}
