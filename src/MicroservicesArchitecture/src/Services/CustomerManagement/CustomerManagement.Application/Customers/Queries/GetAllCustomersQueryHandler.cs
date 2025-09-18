using CustomerManagement.Application.DTOs;
using CustomerManagement.Application.Mappers;
using CustomerManagement.Application.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Application.Common.Queries;
using System.Linq;

namespace CustomerManagement.Application.Customers.Queries;

public class GetAllCustomersQueryHandler : IQueryHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    private readonly ICrudService<Customer> _customerService;
    private readonly ICustomerMapper _customerMapper;

    public GetAllCustomersQueryHandler(ICrudService<Customer> customerService, ICustomerMapper customerMapper)
    {
        _customerService = customerService;
        _customerMapper = customerMapper;
    }

    public async Task<List<CustomerDto>> HandleAsync(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAsync();
        return customers.Select(_customerMapper.MapToDto).ToList();
    }
}
