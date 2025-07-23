using AutoMapper;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Queries;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Application.Mappings;

/// <summary>
/// AutoMapper profile for Order mappings
/// </summary>
public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.CustomerInfo, opt => opt.MapFrom(src => src.CustomerInfo))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<Order, OrderSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerInfo.Name))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count));

        // OrderItem mappings
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

        // Value Object mappings
        CreateMap<Money, MoneyDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency));

        CreateMap<MoneyDto, Money>()
            .ConstructUsing(src => Money.Create(src.Amount, src.Currency));

        CreateMap<Address, AddressDto>()
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));

        CreateMap<AddressDto, Address>()
            .ConstructUsing(src => Address.Create(src.Street, src.City, src.State, src.ZipCode, src.Country));

        CreateMap<CustomerInfo, CustomerInfoDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone));

        CreateMap<CustomerInfoDto, CustomerInfo>()
            .ConstructUsing(src => CustomerInfo.Create(src.Name, src.Email, src.Phone));

        // Statistics mappings
        CreateMap<OrderStatistics, OrderStatisticsDto>()
            .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => new MoneyDto { Amount = src.TotalRevenue, Currency = "USD" }))
            .ForMember(dest => dest.AverageOrderValue, opt => opt.MapFrom(src => new MoneyDto { Amount = src.AverageOrderValue, Currency = "USD" }));

        // Command/Query DTOs to Domain mappings
        CreateMap<CreateOrderDto, Order>()
            .ConstructUsing((src, context) =>
            {
                var customerInfo = context.Mapper.Map<CustomerInfo>(src.CustomerInfo);
                var shippingAddress = src.ShippingAddress != null ? context.Mapper.Map<Address>(src.ShippingAddress) : null;

                var order = Order.Create(src.CustomerId, customerInfo, shippingAddress, src.Notes);

                foreach (var itemDto in src.OrderItems)
                {
                    var unitPrice = Money.Create(itemDto.UnitPrice.Amount, itemDto.UnitPrice.Currency);
                    order.AddOrderItem(itemDto.ProductId, itemDto.ProductName, itemDto.Quantity, unitPrice);
                }

                return order;
            });

        CreateMap<CreateOrderItemDto, OrderItem>()
            .ConstructUsing((src, context) =>
            {
                var unitPrice = Money.Create(src.UnitPrice.Amount, src.UnitPrice.Currency);
                return OrderItem.Create(src.ProductId, src.ProductName, src.Quantity, unitPrice);
            });
    }
}
