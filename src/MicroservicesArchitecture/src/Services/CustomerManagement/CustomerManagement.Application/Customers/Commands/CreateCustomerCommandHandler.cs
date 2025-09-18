using CustomerManagement.Application.DTOs;
using CustomerManagement.Application.Mappers;
using CustomerManagement.Application.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Application.Common.Commands;

namespace CustomerManagement.Application.Customers.Commands;

public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICrudService<Customer> _customerService;
    private readonly ICustomerMapper _customerMapper;

    public CreateCustomerCommandHandler(ICrudService<Customer> customerService, ICustomerMapper customerMapper)
    {
        _customerService = customerService;
        _customerMapper = customerMapper;
    }

    public async Task<CustomerDto> HandleAsync(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = _customerMapper.MapToEntity(request.Customer);
        await _customerService.AddAsync(customer, cancellationToken);
        return _customerMapper.MapToDto(customer);
    }
}
