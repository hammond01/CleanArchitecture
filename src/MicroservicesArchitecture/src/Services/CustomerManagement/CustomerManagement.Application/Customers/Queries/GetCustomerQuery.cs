using CustomerManagement.Application.DTOs;
using CustomerManagement.Application.Common.Queries;

namespace CustomerManagement.Application.Customers.Queries;

public class GetCustomerQuery : IQuery<CustomerDto?>
{
    public string Id { get; set; } = null!;
}
