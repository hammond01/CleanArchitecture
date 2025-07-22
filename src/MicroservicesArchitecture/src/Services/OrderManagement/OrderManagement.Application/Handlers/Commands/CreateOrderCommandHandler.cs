using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Commands;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using OrderManagement.Domain.Services;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Application.Handlers.Commands;

/// <summary>
/// Create order command handler
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderDomainService _orderDomainService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IOrderDomainService orderDomainService,
        IMapper mapper,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _orderDomainService = orderDomainService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);

        try
        {
            // Validate if customer can create orders
            var canCreateOrder = await _orderDomainService.CanCreateOrderAsync(request.CustomerId, cancellationToken);
            if (!canCreateOrder)
            {
                throw new InvalidOperationException($"Customer {request.CustomerId} cannot create orders");
            }

            // Map DTOs to value objects
            var customerInfo = CustomerInfo.Create(
                request.CustomerInfo.Name,
                request.CustomerInfo.Email,
                request.CustomerInfo.Phone);

            Address? shippingAddress = null;
            if (request.ShippingAddress != null)
            {
                shippingAddress = Address.Create(
                    request.ShippingAddress.Street,
                    request.ShippingAddress.City,
                    request.ShippingAddress.State,
                    request.ShippingAddress.ZipCode,
                    request.ShippingAddress.Country);

                // Validate shipping address
                var isValidAddress = await _orderDomainService.ValidateShippingAddressAsync(shippingAddress, cancellationToken);
                if (!isValidAddress)
                {
                    throw new ArgumentException("Invalid shipping address");
                }
            }

            // Create order
            var order = Order.Create(
                request.CustomerId,
                customerInfo,
                shippingAddress,
                request.Notes);

            // Add order items
            foreach (var itemDto in request.OrderItems)
            {
                var unitPrice = Money.Create(itemDto.UnitPrice.Amount, itemDto.UnitPrice.Currency);
                order.AddOrderItem(itemDto.ProductId, itemDto.ProductName, itemDto.Quantity, unitPrice);
            }

            // Validate order items availability
            var itemsAvailable = await _orderDomainService.ValidateOrderItemsAvailabilityAsync(order, cancellationToken);
            if (!itemsAvailable)
            {
                throw new InvalidOperationException("Some order items are not available");
            }

            // Begin transaction
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Save order
                var savedOrder = await _unitOfWork.Orders.AddAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerId}", 
                    savedOrder.Id, request.CustomerId);

                // Map to DTO and return
                return _mapper.Map<OrderDto>(savedOrder);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for customer {CustomerId}", request.CustomerId);
            throw;
        }
    }
}
