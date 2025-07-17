using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Data;
using OrderManagement.API.DTOs;
using OrderManagement.API.Models;

namespace OrderManagement.API.Services;

public class CustomerService : ICustomerService
{
    private readonly OrderDbContext _context;

    public CustomerService(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerSummaryDto>> GetCustomersAsync(int pageNumber = 1, int pageSize = 10)
    {
        var customers = await _context.Customers
            .Include(c => c.Orders)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CustomerSummaryDto
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                City = c.City,
                State = c.State,
                TotalOrders = c.Orders.Count,
                TotalSpent = c.Orders.Sum(o => o.TotalAmount),
                LastOrderDate = c.Orders.OrderByDescending(o => o.OrderDate).FirstOrDefault()!.OrderDate
            })
            .ToListAsync();

        return customers;
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (customer == null)
            return null;

        return MapToCustomerDto(customer);
    }

    public async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
    {
        var customer = await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Email == email);

        if (customer == null)
            return null;

        return MapToCustomerDto(customer);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
    {
        // Check if email already exists
        var existingCustomer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == createCustomerDto.Email);

        if (existingCustomer != null)
            throw new ArgumentException($"Customer with email {createCustomerDto.Email} already exists");

        var customer = new Customer
        {
            FirstName = createCustomerDto.FirstName,
            LastName = createCustomerDto.LastName,
            Email = createCustomerDto.Email,
            Phone = createCustomerDto.Phone,
            Address = createCustomerDto.Address,
            City = createCustomerDto.City,
            State = createCustomerDto.State,
            ZipCode = createCustomerDto.ZipCode,
            Country = createCustomerDto.Country,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return MapToCustomerDto(customer);
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(int customerId, UpdateCustomerDto updateCustomerDto)
    {
        var customer = await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (customer == null)
            return null;

        // Update fields
        if (!string.IsNullOrEmpty(updateCustomerDto.FirstName))
            customer.FirstName = updateCustomerDto.FirstName;

        if (!string.IsNullOrEmpty(updateCustomerDto.LastName))
            customer.LastName = updateCustomerDto.LastName;

        if (!string.IsNullOrEmpty(updateCustomerDto.Phone))
            customer.Phone = updateCustomerDto.Phone;

        if (!string.IsNullOrEmpty(updateCustomerDto.Address))
            customer.Address = updateCustomerDto.Address;

        if (!string.IsNullOrEmpty(updateCustomerDto.City))
            customer.City = updateCustomerDto.City;

        if (!string.IsNullOrEmpty(updateCustomerDto.State))
            customer.State = updateCustomerDto.State;

        if (!string.IsNullOrEmpty(updateCustomerDto.ZipCode))
            customer.ZipCode = updateCustomerDto.ZipCode;

        if (!string.IsNullOrEmpty(updateCustomerDto.Country))
            customer.Country = updateCustomerDto.Country;

        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToCustomerDto(customer);
    }

    public async Task<bool> DeleteCustomerAsync(int customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (customer == null)
            return false;

        // Don't allow deletion if customer has orders
        if (customer.Orders.Any())
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }

    private static CustomerDto MapToCustomerDto(Customer customer)
    {
        return new CustomerDto
        {
            CustomerId = customer.CustomerId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
            State = customer.State,
            ZipCode = customer.ZipCode,
            Country = customer.Country,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            FullName = customer.FullName,
            TotalOrders = customer.Orders.Count,
            TotalSpent = customer.Orders.Sum(o => o.TotalAmount)
        };
    }
}
