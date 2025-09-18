using CustomerManagement.Application.DTOs;
using CustomerManagement.Application.Common.Commands;

namespace CustomerManagement.Application.Customers.Commands;

public class CreateCustomerCommand : ICommand<CustomerDto>
{
    public CreateCustomerRequest Customer { get; set; } = null!;
}
