using CustomerManagement.Application.Common.Commands;

namespace CustomerManagement.Application.Customers.Commands;

public class DeleteCustomerCommand : ICommand<bool>
{
    public string Id { get; set; } = null!;
}
