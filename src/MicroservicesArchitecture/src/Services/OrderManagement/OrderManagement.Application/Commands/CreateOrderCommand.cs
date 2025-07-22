using MediatR;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Commands;

/// <summary>
/// Create order command
/// </summary>
public class CreateOrderCommand : IRequest<OrderDto>
{
    public string CustomerId { get; set; } = string.Empty;
    public CustomerInfoDto CustomerInfo { get; set; } = null!;
    public AddressDto? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();

    public CreateOrderCommand() { }

    public CreateOrderCommand(CreateOrderDto dto)
    {
        CustomerId = dto.CustomerId;
        CustomerInfo = dto.CustomerInfo;
        ShippingAddress = dto.ShippingAddress;
        Notes = dto.Notes;
        OrderItems = dto.OrderItems;
    }
}
