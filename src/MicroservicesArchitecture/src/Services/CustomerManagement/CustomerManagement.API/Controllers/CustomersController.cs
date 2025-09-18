using Microsoft.AspNetCore.Mvc;
using CustomerManagement.Application.Customers.Commands;
using CustomerManagement.Application.Customers.Queries;
using CustomerManagement.Application.DTOs;
using CustomerManagement.Application.Common;
using System.Collections.Generic;

namespace CustomerManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly MessageDispatcher _dispatcher;

    public CustomersController(MessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(string id)
    {
        var query = new GetCustomerQuery { Id = id };
        var customer = await _dispatcher.DispatchAsync<CustomerDto?>(query);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerDto>>> GetAllCustomers()
    {
        var query = new GetAllCustomersQuery();
        var customers = await _dispatcher.DispatchAsync<List<CustomerDto>>(query);
        return Ok(customers);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(string id, CreateCustomerRequest request)
    {
        var command = new UpdateCustomerCommand { Id = id, Customer = request };
        var customer = await _dispatcher.DispatchAsync<CustomerDto>(command);
        return Ok(customer);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(string id)
    {
        var command = new DeleteCustomerCommand { Id = id };
        var result = await _dispatcher.DispatchAsync<bool>(command);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
