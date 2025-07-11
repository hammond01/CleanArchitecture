using Shared.Contracts.Common;

namespace Shared.Contracts.Orders;

/// <summary>
/// Order data transfer object
/// </summary>
public class OrderDto
{
    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public int? ShipVia { get; set; }
    public decimal? Freight { get; set; }
    public string? ShipName { get; set; }
    public string? ShipAddress { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipRegion { get; set; }
    public string? ShipPostalCode { get; set; }
    public string? ShipCountry { get; set; }
    public string? CustomerName { get; set; }
    public string? EmployeeName { get; set; }
    public string? ShipperName { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}

/// <summary>
/// Order detail data transfer object
/// </summary>
public class OrderDetailDto
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public short Quantity { get; set; }
    public float Discount { get; set; }
    public string? ProductName { get; set; }
}

/// <summary>
/// Create order request
/// </summary>
public class CreateOrderRequest : IRequest<ApiResponse<OrderDto>>
{
    public string? CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public int? ShipVia { get; set; }
    public decimal? Freight { get; set; }
    public string? ShipName { get; set; }
    public string? ShipAddress { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipRegion { get; set; }
    public string? ShipPostalCode { get; set; }
    public string? ShipCountry { get; set; }
    public List<CreateOrderDetailRequest> OrderDetails { get; set; } = new();
}

/// <summary>
/// Create order detail request
/// </summary>
public class CreateOrderDetailRequest
{
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public short Quantity { get; set; }
    public float Discount { get; set; }
}

/// <summary>
/// Update order request
/// </summary>
public class UpdateOrderRequest : IRequest<ApiResponse<OrderDto>>
{
    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public int? ShipVia { get; set; }
    public decimal? Freight { get; set; }
    public string? ShipName { get; set; }
    public string? ShipAddress { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipRegion { get; set; }
    public string? ShipPostalCode { get; set; }
    public string? ShipCountry { get; set; }
}

/// <summary>
/// Get order by ID request
/// </summary>
public class GetOrderByIdRequest : IRequest<ApiResponse<OrderDto>>
{
    public int OrderId { get; set; }
}

/// <summary>
/// Get orders request
/// </summary>
public class GetOrdersRequest : IRequest<ApiResponse<List<OrderDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? ShipCountry { get; set; }
}

/// <summary>
/// Delete order request
/// </summary>
public class DeleteOrderRequest : IRequest<ApiResponse>
{
    public int OrderId { get; set; }
}
