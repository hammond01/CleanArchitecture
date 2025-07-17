using OrderManagement.API.DTOs;

namespace OrderManagement.API.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerSummaryDto>> GetCustomersAsync(int pageNumber = 1, int pageSize = 10);
    Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
    Task<CustomerDto?> GetCustomerByEmailAsync(string email);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
    Task<CustomerDto?> UpdateCustomerAsync(int customerId, UpdateCustomerDto updateCustomerDto);
    Task<bool> DeleteCustomerAsync(int customerId);
}
