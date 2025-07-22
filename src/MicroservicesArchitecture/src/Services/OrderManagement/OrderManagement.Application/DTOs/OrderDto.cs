using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.DTOs;

/// <summary>
/// Order DTO for responses
/// </summary>
public class OrderDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public CustomerInfoDto CustomerInfo { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public MoneyDto TotalAmount { get; set; } = null!;
    public AddressDto? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}

/// <summary>
/// Order summary DTO for list views
/// </summary>
public class OrderSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public MoneyDto TotalAmount { get; set; } = null!;
    public int ItemCount { get; set; }
}

/// <summary>
/// Order item DTO
/// </summary>
public class OrderItemDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public MoneyDto UnitPrice { get; set; } = null!;
    public MoneyDto TotalPrice { get; set; } = null!;
}

/// <summary>
/// Money DTO
/// </summary>
public class MoneyDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }
}

/// <summary>
/// Address DTO
/// </summary>
public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string GetFullAddress()
    {
        return $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }
}

/// <summary>
/// Customer info DTO
/// </summary>
public class CustomerInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

/// <summary>
/// Create order DTO
/// </summary>
public class CreateOrderDto
{
    public string CustomerId { get; set; } = string.Empty;
    public CustomerInfoDto CustomerInfo { get; set; } = null!;
    public AddressDto? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

/// <summary>
/// Create order item DTO
/// </summary>
public class CreateOrderItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public MoneyDto UnitPrice { get; set; } = null!;
}

/// <summary>
/// Update order DTO
/// </summary>
public class UpdateOrderDto
{
    public AddressDto? ShippingAddress { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Update order status DTO
/// </summary>
public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Add order item DTO
/// </summary>
public class AddOrderItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public MoneyDto UnitPrice { get; set; } = null!;
}

/// <summary>
/// Update order item DTO
/// </summary>
public class UpdateOrderItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

/// <summary>
/// Paged result DTO
/// </summary>
public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
