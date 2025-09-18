using CustomerManagement.Application.DTOs;
using CustomerManagement.Application.Mappers;
using CustomerManagement.Application.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Application.Common.Commands;
using System.Collections.Generic;

namespace CustomerManagement.Application.Customers.Commands;

public class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomerCommand, CustomerDto>
{
    private readonly ICrudService<Customer> _customerService;
    private readonly ICustomerMapper _customerMapper;

    public UpdateCustomerCommandHandler(ICrudService<Customer> customerService, ICustomerMapper customerMapper)
    {
        _customerService = customerService;
        _customerMapper = customerMapper;
    }

    public async Task<CustomerDto> HandleAsync(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existingCustomer = await _customerService.GetByIdAsync(request.Id);
        if (existingCustomer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {request.Id} not found");
        }

        _customerMapper.MapToEntity(request.Customer, existingCustomer);
        await _customerService.UpdateAsync(existingCustomer, cancellationToken);
        return _customerMapper.MapToDto(existingCustomer);
    }
}
