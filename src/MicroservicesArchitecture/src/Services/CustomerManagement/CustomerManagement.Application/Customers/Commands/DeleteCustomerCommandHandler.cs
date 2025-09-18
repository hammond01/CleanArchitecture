using CustomerManagement.Application.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Application.Common.Commands;
using System.Collections.Generic;

namespace CustomerManagement.Application.Customers.Commands;

public class DeleteCustomerCommandHandler : ICommandHandler<DeleteCustomerCommand, bool>
{
    private readonly ICrudService<Customer> _customerService;

    public DeleteCustomerCommandHandler(ICrudService<Customer> customerService)
    {
        _customerService = customerService;
    }

    public async Task<bool> HandleAsync(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(request.Id);
        if (customer == null)
        {
            return false;
        }

        await _customerService.DeleteAsync(customer, cancellationToken);
        return true;
    }
}
