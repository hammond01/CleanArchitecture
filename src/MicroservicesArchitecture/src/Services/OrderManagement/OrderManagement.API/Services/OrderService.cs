using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Data;
using OrderManagement.API.DTOs;
using OrderManagement.API.Models;

namespace OrderManagement.API.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;

    public OrderService(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetOrdersAsync(int pageNumber = 1, int pageSize = 10)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderSummaryDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                CustomerName = o.CustomerName,
                OrderDate = o.OrderDate,
                Status = o.Status,
                StatusName = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                ItemCount = o.OrderItems.Count
            })
            .ToListAsync();

        return orders;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return null;

        return MapToOrderDto(order);
    }

    public async Task<OrderDto?> GetOrderByNumberAsync(string orderNumber)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        if (order == null)
            return null;

        return MapToOrderDto(order);
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerIdAsync(int customerId)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderSummaryDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                CustomerName = o.CustomerName,
                OrderDate = o.OrderDate,
                Status = o.Status,
                StatusName = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                ItemCount = o.OrderItems.Count
            })
            .ToListAsync();

        return orders;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        // Get customer info
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == createOrderDto.CustomerId);

        if (customer == null)
            throw new ArgumentException($"Customer with ID {createOrderDto.CustomerId} not found");

        // Generate order number
        var orderNumber = await GenerateOrderNumberAsync();

        // Calculate totals
        var totalAmount = createOrderDto.OrderItems.Sum(item => item.Quantity * item.UnitPrice);

        // Create order
        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerId = createOrderDto.CustomerId,
            CustomerName = customer.FullName,
            CustomerEmail = customer.Email,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = totalAmount,
            ShippingAddress = createOrderDto.ShippingAddress ?? $"{customer.Address}, {customer.City}, {customer.State} {customer.ZipCode}",
            Notes = createOrderDto.Notes,
            CreatedAt = DateTime.UtcNow,
            OrderItems = createOrderDto.OrderItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Reload with items
        await _context.Entry(order)
            .Collection(o => o.OrderItems)
            .LoadAsync();

        return MapToOrderDto(order);
    }

    public async Task<OrderDto?> UpdateOrderAsync(int orderId, UpdateOrderDto updateOrderDto)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return null;

        // Update fields
        if (updateOrderDto.Status.HasValue)
            order.Status = updateOrderDto.Status.Value;

        if (!string.IsNullOrEmpty(updateOrderDto.ShippingAddress))
            order.ShippingAddress = updateOrderDto.ShippingAddress;

        if (!string.IsNullOrEmpty(updateOrderDto.Notes))
            order.Notes = updateOrderDto.Notes;

        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToOrderDto(order);
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto updateStatusDto)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return null;

        order.Status = updateStatusDto.Status;
        
        if (!string.IsNullOrEmpty(updateStatusDto.Notes))
            order.Notes = updateStatusDto.Notes;

        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToOrderDto(order);
    }

    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return false;

        // Only allow deletion of pending orders
        if (order.Status != OrderStatus.Pending)
            return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CancelOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return false;

        // Only allow cancellation of certain statuses
        var cancellableStatuses = new[] { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.Processing };
        if (!cancellableStatuses.Contains(order.Status))
            return false;

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var now = DateTime.UtcNow;
        var prefix = $"ORD-{now.Year}-";
        
        var lastOrder = await _context.Orders
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .OrderByDescending(o => o.OrderNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastOrder != null)
        {
            var lastNumberPart = lastOrder.OrderNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumberPart, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D3}";
    }

    private static OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            OrderDate = order.OrderDate,
            Status = order.Status,
            StatusName = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            OrderItems = order.OrderItems.Select(item => new OrderItemDto
            {
                OrderItemId = item.OrderItemId,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList()
        };
    }
}
