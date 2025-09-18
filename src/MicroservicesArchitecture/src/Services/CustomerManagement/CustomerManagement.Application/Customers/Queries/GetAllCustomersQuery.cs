using CustomerManagement.Application.DTOs;
using CustomerManagement.Application.Common.Queries;

namespace CustomerManagement.Application.Customers.Queries;

public class GetAllCustomersQuery : IQuery<List<CustomerDto>>
{
}
