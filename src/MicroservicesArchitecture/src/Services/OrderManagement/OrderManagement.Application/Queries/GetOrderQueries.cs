using MediatR;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Queries;

/// <summary>
/// Get order by ID query
/// </summary>
public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public string OrderId { get; set; } = string.Empty;

    public GetOrderByIdQuery() { }

    public GetOrderByIdQuery(string orderId)
    {
        OrderId = orderId;
    }
}

/// <summary>
/// Get order by order number query
/// </summary>
public class GetOrderByOrderNumberQuery : IRequest<OrderDto?>
{
    public string OrderNumber { get; set; } = string.Empty;

    public GetOrderByOrderNumberQuery() { }

    public GetOrderByOrderNumberQuery(string orderNumber)
    {
        OrderNumber = orderNumber;
    }
}

/// <summary>
/// Get orders query with pagination
/// </summary>
public class GetOrdersQuery : IRequest<PagedResultDto<OrderSummaryDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public OrderStatus? Status { get; set; }
    public string? CustomerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public GetOrdersQuery() { }

    public GetOrdersQuery(int pageNumber, int pageSize, OrderStatus? status = null, string? customerId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Status = status;
        CustomerId = customerId;
        StartDate = startDate;
        EndDate = endDate;
    }
}

/// <summary>
/// Get orders by customer ID query
/// </summary>
public class GetOrdersByCustomerIdQuery : IRequest<IEnumerable<OrderSummaryDto>>
{
    public string CustomerId { get; set; } = string.Empty;

    public GetOrdersByCustomerIdQuery() { }

    public GetOrdersByCustomerIdQuery(string customerId)
    {
        CustomerId = customerId;
    }
}

/// <summary>
/// Get orders by status query
/// </summary>
public class GetOrdersByStatusQuery : IRequest<IEnumerable<OrderSummaryDto>>
{
    public OrderStatus Status { get; set; }

    public GetOrdersByStatusQuery() { }

    public GetOrdersByStatusQuery(OrderStatus status)
    {
        Status = status;
    }
}

/// <summary>
/// Get orders by date range query
/// </summary>
public class GetOrdersByDateRangeQuery : IRequest<IEnumerable<OrderSummaryDto>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public GetOrdersByDateRangeQuery() { }

    public GetOrdersByDateRangeQuery(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}

/// <summary>
/// Get order statistics query
/// </summary>
public class GetOrderStatisticsQuery : IRequest<OrderStatisticsDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public GetOrderStatisticsQuery() { }

    public GetOrderStatisticsQuery(DateTime? startDate = null, DateTime? endDate = null)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}

/// <summary>
/// Order statistics DTO
/// </summary>
public class OrderStatisticsDto
{
    public int TotalOrders { get; set; }
    public MoneyDto TotalRevenue { get; set; } = null!;
    public int PendingOrders { get; set; }
    public int ConfirmedOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int CancelledOrders { get; set; }
    public MoneyDto AverageOrderValue { get; set; } = null!;
}
