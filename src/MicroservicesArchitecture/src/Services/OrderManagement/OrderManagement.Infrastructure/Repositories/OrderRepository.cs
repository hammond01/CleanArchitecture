using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories;

/// <summary>
/// Order repository implementation
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly OrderManagementDbContext _context;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(OrderManagementDbContext context, ILogger<OrderRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Order?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting order by ID: {OrderId}", id);

        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting order by order number: {OrderNumber}", orderNumber);

        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting orders for customer: {CustomerId}", customerId);

        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        OrderStatus? status = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting paged orders - Page: {PageNumber}, Size: {PageSize}, Status: {Status}", 
            pageNumber, pageSize, status);

        var query = _context.Orders.Include(o => o.OrderItems).AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (orders, totalCount);
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting orders by status: {Status}", status);

        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting orders by date range: {StartDate} to {EndDate}", startDate, endDate);

        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding new order: {OrderId}", order.Id);

        var entityEntry = await _context.Orders.AddAsync(order, cancellationToken);
        return entityEntry.Entity;
    }

    public async Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating order: {OrderId}", order.Id);

        _context.Orders.Update(order);
        return await Task.FromResult(order);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting order: {OrderId}", id);

        var order = await GetByIdAsync(id, cancellationToken);
        if (order != null)
        {
            _context.Orders.Remove(order);
        }
    }

    public async Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if order number exists: {OrderNumber}", orderNumber);

        return await _context.Orders
            .AnyAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<OrderStatistics> GetStatisticsAsync(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting order statistics for date range: {StartDate} to {EndDate}", startDate, endDate);

        var query = _context.Orders.AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= endDate.Value);
        }

        var totalOrders = await query.CountAsync(cancellationToken);
        var totalRevenue = await query.SumAsync(o => o.TotalAmount.Amount, cancellationToken);

        var statusCounts = await query
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

        return new OrderStatistics
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            PendingOrders = statusCounts.FirstOrDefault(s => s.Status == OrderStatus.Pending)?.Count ?? 0,
            ConfirmedOrders = statusCounts.FirstOrDefault(s => s.Status == OrderStatus.Confirmed)?.Count ?? 0,
            ProcessingOrders = statusCounts.FirstOrDefault(s => s.Status == OrderStatus.Processing)?.Count ?? 0,
            ShippedOrders = statusCounts.FirstOrDefault(s => s.Status == OrderStatus.Shipped)?.Count ?? 0,
            DeliveredOrders = statusCounts.FirstOrDefault(s => s.Status == OrderStatus.Delivered)?.Count ?? 0,
            CancelledOrders = statusCounts.FirstOrDefault(s => s.Status == OrderStatus.Cancelled)?.Count ?? 0,
            AverageOrderValue = averageOrderValue
        };
    }
}
