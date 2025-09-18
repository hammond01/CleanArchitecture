using CustomerManagement.Application.DTOs;
using CustomerManagement.Application.Common.Commands;

namespace CustomerManagement.Application.Customers.Commands;

public class UpdateCustomerCommand : ICommand<CustomerDto>
{
    public string Id { get; set; } = null!;
    public CreateCustomerRequest Customer { get; set; } = null!;
}
