using Shared.Contracts.Common;

namespace Shared.Contracts.Customers;

/// <summary>
/// Customer data transfer object
/// </summary>
public class CustomerDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
}

/// <summary>
/// Create customer request
/// </summary>
public class CreateCustomerRequest : IRequest<ApiResponse<CustomerDto>>
{
    public string CustomerId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
}

/// <summary>
/// Update customer request
/// </summary>
public class UpdateCustomerRequest : IRequest<ApiResponse<CustomerDto>>
{
    public string CustomerId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
}

/// <summary>
/// Get customer by ID request
/// </summary>
public class GetCustomerByIdRequest : IRequest<ApiResponse<CustomerDto>>
{
    public string CustomerId { get; set; } = string.Empty;
}

/// <summary>
/// Get customers request
/// </summary>
public class GetCustomersRequest : IRequest<ApiResponse<List<CustomerDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
}

/// <summary>
/// Delete customer request
/// </summary>
public class DeleteCustomerRequest : IRequest<ApiResponse>
{
    public string CustomerId { get; set; } = string.Empty;
}
