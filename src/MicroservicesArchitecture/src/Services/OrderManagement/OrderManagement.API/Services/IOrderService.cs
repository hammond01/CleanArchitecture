using OrderManagement.API.DTOs;
using OrderManagement.API.Models;

namespace OrderManagement.API.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderSummaryDto>> GetOrdersAsync(int pageNumber = 1, int pageSize = 10);
    Task<OrderDto?> GetOrderByIdAsync(int orderId);
    Task<OrderDto?> GetOrderByNumberAsync(string orderNumber);
    Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerIdAsync(int customerId);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
    Task<OrderDto?> UpdateOrderAsync(int orderId, UpdateOrderDto updateOrderDto);
    Task<OrderDto?> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto updateStatusDto);
    Task<bool> DeleteOrderAsync(int orderId);
    Task<bool> CancelOrderAsync(int orderId);
}
