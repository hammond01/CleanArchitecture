using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Commands;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Repositories;
using OrderManagement.Domain.Services;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Application.Handlers.Commands;

/// <summary>
/// Update order command handler
/// </summary>
public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderDomainService _orderDomainService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;

    public UpdateOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IOrderDomainService orderDomainService,
        IMapper mapper,
        ILogger<UpdateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _orderDomainService = orderDomainService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating order {OrderId}", request.OrderId);

        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", request.OrderId);
                return null;
            }

            // Check if order can be modified
            if (!_orderDomainService.CanModifyOrder(order))
            {
                throw new InvalidOperationException($"Order {request.OrderId} cannot be modified in its current status");
            }

            // Update shipping address if provided
            if (request.ShippingAddress != null)
            {
                var shippingAddress = Address.Create(
                    request.ShippingAddress.Street,
                    request.ShippingAddress.City,
                    request.ShippingAddress.State,
                    request.ShippingAddress.ZipCode,
                    request.ShippingAddress.Country);

                var isValidAddress = await _orderDomainService.ValidateShippingAddressAsync(shippingAddress, cancellationToken);
                if (!isValidAddress)
                {
                    throw new ArgumentException("Invalid shipping address");
                }

                order.UpdateShippingAddress(shippingAddress);
            }

            // Update notes
            if (request.Notes != null)
            {
                order.UpdateNotes(request.Notes);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var updatedOrder = await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Order {OrderId} updated successfully", request.OrderId);
                return _mapper.Map<OrderDto>(updatedOrder);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId}", request.OrderId);
            throw;
        }
    }
}

/// <summary>
/// Update order status command handler
/// </summary>
public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating order {OrderId} status to {Status}", request.OrderId, request.Status);

        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", request.OrderId);
                return null;
            }

            // Update status based on the requested status
            switch (request.Status)
            {
                case Domain.Entities.OrderStatus.Confirmed:
                    order.ConfirmOrder();
                    break;
                case Domain.Entities.OrderStatus.Processing:
                    order.StartProcessing();
                    break;
                case Domain.Entities.OrderStatus.Cancelled:
                    order.Cancel(request.Notes ?? "Order cancelled");
                    break;
                default:
                    throw new ArgumentException($"Cannot directly set order status to {request.Status}");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var updatedOrder = await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Order {OrderId} status updated to {Status}", request.OrderId, request.Status);
                return _mapper.Map<OrderDto>(updatedOrder);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId} status", request.OrderId);
            throw;
        }
    }
}
