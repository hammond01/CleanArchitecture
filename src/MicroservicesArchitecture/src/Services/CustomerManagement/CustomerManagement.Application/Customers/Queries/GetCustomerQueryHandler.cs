using CustomerManagement.Application.DTOs;
using CustomerManagement.Application.Mappers;
using CustomerManagement.Application.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Application.Common.Queries;

namespace CustomerManagement.Application.Customers.Queries;

public class GetCustomerQueryHandler : IQueryHandler<GetCustomerQuery, CustomerDto?>
{
    private readonly ICrudService<Customer> _customerService;
    private readonly ICustomerMapper _customerMapper;

    public GetCustomerQueryHandler(ICrudService<Customer> customerService, ICustomerMapper customerMapper)
    {
        _customerService = customerService;
        _customerMapper = customerMapper;
    }

    public async Task<CustomerDto?> HandleAsync(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(request.Id);
        return customer != null ? _customerMapper.MapToDto(customer) : null;
    }
}
