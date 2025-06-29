using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Customer.Commands;
using ProductManager.Application.Feature.Customer.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Infrastructure.Middleware;
using ProductManager.Shared.DTOs.CustomerDto;
namespace ProductManager.Api.Controllers;

/// <summary>
///     Customer management endpoints with full CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customers")]
public class CustomerController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public CustomerController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    ///     Get all customers with OData support
    /// </summary>
    [HttpGet]
    [LogAction("Get all customers")]
    public async Task<ActionResult<ApiResponse>> GetCustomers()
    {
        var data = await _dispatcher.DispatchAsync(new GetCustomers());
        data.Result = data.Result.Adapt<List<GetCustomerDto>>();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [LogAction("Get customer by ID")]
    public async Task<ActionResult<ApiResponse>> GetCustomer(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetCustomerByIdQuery(id));
        data.Result = data.Result.Adapt<GetCustomerDto>();
        return Ok(data);
    }

    [HttpPost]
    [LogAction("Create new customer")]
    public async Task<ActionResult<ApiResponse>> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto)
    {
        var customer = createCustomerDto.Adapt<Customers>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateCustomerCommand(customer));
        var createdCustomer = (Customers)result.Result;
        return Created($"/api/v1.0/customers/{createdCustomer.Id}", result);
    }

    [HttpPut("{id}")]
    [EntityLock("Customer", "id", 30)]
    [LogAction("Update customer")]
    public async Task<ActionResult<ApiResponse>> UpdateCustomer(string id, [FromBody] UpdateCustomerDto updateCustomerDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetCustomerByIdQuery(id));
        if (apiResponse.Result is not Customers customer)
        {
            return NotFound(new ApiResponse(404, "Customer not found"));
        }
        updateCustomerDto.Adapt(customer);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateCustomerCommand(customer));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [EntityLock("Customer", "id", 30)]
    [LogAction("Delete customer")]
    public async Task<ActionResult<ApiResponse>> DeleteCustomer(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetCustomerByIdQuery(id));
        if (apiResponse.Result is not Customers customer)
        {
            return NotFound(new ApiResponse(404, "Customer not found"));
        }
        await _dispatcher.DispatchAsync(new DeleteCustomerCommand(customer));
        return NoContent();
    }
}
