using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Repositories;

/// <summary>
/// Order repository interface
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Get order by ID
    /// </summary>
    Task<Order?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get order by order number
    /// </summary>
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get orders by customer ID
    /// </summary>
    Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get orders with pagination
    /// </summary>
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        OrderStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get orders by status
    /// </summary>
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get orders by date range
    /// </summary>
    Task<IEnumerable<Order>> GetByDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add new order
    /// </summary>
    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing order
    /// </summary>
    Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete order
    /// </summary>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if order number exists
    /// </summary>
    Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get order statistics
    /// </summary>
    Task<OrderStatistics> GetStatisticsAsync(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Order statistics
/// </summary>
public class OrderStatistics
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingOrders { get; set; }
    public int ConfirmedOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
}
