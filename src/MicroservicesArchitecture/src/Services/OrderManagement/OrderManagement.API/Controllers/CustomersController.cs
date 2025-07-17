using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.DTOs;
using OrderManagement.API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all customers with pagination")]
    [SwaggerResponse(200, "Returns list of customers", typeof(IEnumerable<CustomerSummaryDto>))]
    public async Task<ActionResult<IEnumerable<CustomerSummaryDto>>> GetCustomers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var customers = await _customerService.GetCustomersAsync(pageNumber, pageSize);
        return Ok(customers);
    }

    [HttpGet("{customerId:int}")]
    [SwaggerOperation(Summary = "Get customer by ID")]
    [SwaggerResponse(200, "Returns customer details", typeof(CustomerDto))]
    [SwaggerResponse(404, "Customer not found")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int customerId)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        
        if (customer == null)
            return NotFound($"Customer with ID {customerId} not found");

        return Ok(customer);
    }

    [HttpGet("by-email/{email}")]
    [SwaggerOperation(Summary = "Get customer by email")]
    [SwaggerResponse(200, "Returns customer details", typeof(CustomerDto))]
    [SwaggerResponse(404, "Customer not found")]
    public async Task<ActionResult<CustomerDto>> GetCustomerByEmail(string email)
    {
        var customer = await _customerService.GetCustomerByEmailAsync(email);
        
        if (customer == null)
            return NotFound($"Customer with email {email} not found");

        return Ok(customer);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new customer")]
    [SwaggerResponse(201, "Customer created successfully", typeof(CustomerDto))]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto)
    {
        try
        {
            var customer = await _customerService.CreateCustomerAsync(createCustomerDto);
            return CreatedAtAction(nameof(GetCustomer), new { customerId = customer.CustomerId }, customer);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{customerId:int}")]
    [SwaggerOperation(Summary = "Update an existing customer")]
    [SwaggerResponse(200, "Customer updated successfully", typeof(CustomerDto))]
    [SwaggerResponse(404, "Customer not found")]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(int customerId, [FromBody] UpdateCustomerDto updateCustomerDto)
    {
        var customer = await _customerService.UpdateCustomerAsync(customerId, updateCustomerDto);
        
        if (customer == null)
            return NotFound($"Customer with ID {customerId} not found");

        return Ok(customer);
    }

    [HttpDelete("{customerId:int}")]
    [SwaggerOperation(Summary = "Delete a customer")]
    [SwaggerResponse(200, "Customer deleted successfully")]
    [SwaggerResponse(404, "Customer not found")]
    [SwaggerResponse(400, "Customer cannot be deleted")]
    public async Task<ActionResult> DeleteCustomer(int customerId)
    {
        var success = await _customerService.DeleteCustomerAsync(customerId);
        
        if (!success)
            return BadRequest("Customer cannot be deleted or does not exist");

        return Ok(new { message = "Customer deleted successfully" });
    }
}
