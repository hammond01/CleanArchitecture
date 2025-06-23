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
[Route("api/[controller]")]
public class CustomerController : ConBase
{
    private readonly Dispatcher _dispatcher;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(Dispatcher dispatcher, ILogger<CustomerController> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    /// <summary>
    ///     Get all customers with OData support
    /// </summary>
    [HttpGet]
    [LogAction("Get all customers")]
    public async Task<ApiResponse> Get()
    {
        _logger.LogInformation("🔍 Getting all customers");
        var data = await _dispatcher.DispatchAsync(new GetCustomers());
        data.Result = ((List<Customers>)data.Result).Adapt<List<GetCustomerDto>>();
        _logger.LogInformation("✅ Retrieved {Count} customers", ((List<GetCustomerDto>)data.Result).Count);
        return data;
    }

    /// <summary>
    ///     Get customer by ID
    /// </summary>
    [HttpGet("{id}")]
    [LogAction("Get customer by ID")]
    public async Task<ApiResponse> Get(string id)
    {
        _logger.LogInformation("🔍 Getting customer with ID: {CustomerId}", id);
        var data = await _dispatcher.DispatchAsync(new GetCustomerByIdQuery(id));
        data.Result = ((Customers)data.Result).Adapt<GetCustomerDto>();
        _logger.LogInformation("✅ Retrieved customer: {CompanyName}", ((GetCustomerDto)data.Result).CompanyName);
        return data;
    }

    /// <summary>
    ///     Create a new customer
    /// </summary>
    [HttpPost]
    [LogAction("Create new customer")]
    public async Task<ApiResponse> Post([FromBody] CreateCustomerDto createCustomerDto)
    {
        _logger.LogInformation("➕ Creating new customer: {CompanyName}", createCustomerDto.CompanyName);
        var customer = createCustomerDto.Adapt<Customers>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateCustomerCommand(customer));
        _logger.LogInformation("✅ Customer created with status: {StatusCode}", result.StatusCode);
        return result;
    }

    /// <summary>
    ///     Update an existing customer
    /// </summary>
    [HttpPut("{id}")]
    [EntityLock("Customer", "id", 30)]
    [LogAction("Update customer")]
    public async Task<ApiResponse> Put(string id, [FromBody] UpdateCustomerDto updateCustomerDto)
    {
        _logger.LogInformation("✏️ Updating customer with ID: {CustomerId}", id);
        var apiResponse = await _dispatcher.DispatchAsync(new GetCustomerByIdQuery(id));

        var customer = (Customers)apiResponse.Result;
        updateCustomerDto.Adapt(customer);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateCustomerCommand(customer));
        _logger.LogInformation("✅ Customer updated with status: {StatusCode}", result.StatusCode);
        return result;
    }

    /// <summary>
    ///     Delete a customer
    /// </summary>
    [HttpDelete("{id}")]
    [EntityLock("Customer", "id", 30)]
    [LogAction("Delete customer")]
    public async Task<ApiResponse> Delete(string id)
    {
        _logger.LogInformation("🗑️ Deleting customer with ID: {CustomerId}", id);
        var apiResponse = await _dispatcher.DispatchAsync(new GetCustomerByIdQuery(id));

        var customer = (Customers)apiResponse.Result;
        var result = await _dispatcher.DispatchAsync(new DeleteCustomerCommand(customer));
        _logger.LogInformation("✅ Customer deleted with status: {StatusCode}", result.StatusCode);
        return result;
    }
}
