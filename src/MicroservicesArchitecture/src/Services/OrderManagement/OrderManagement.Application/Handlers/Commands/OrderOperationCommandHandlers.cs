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
/// Confirm order command handler
/// </summary>
public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ConfirmOrderCommandHandler> _logger;

    public ConfirmOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ConfirmOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Confirming order {OrderId}", request.OrderId);

        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", request.OrderId);
            return null;
        }

        order.ConfirmOrder();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var updatedOrder = await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} confirmed successfully", request.OrderId);
            return _mapper.Map<OrderDto>(updatedOrder);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

/// <summary>
/// Cancel order command handler
/// </summary>
public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderDomainService _orderDomainService;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IOrderDomainService orderDomainService,
        IMapper mapper,
        ILogger<CancelOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _orderDomainService = orderDomainService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling order {OrderId}", request.OrderId);

        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", request.OrderId);
            return null;
        }

        if (!_orderDomainService.CanCancelOrder(order))
        {
            throw new InvalidOperationException($"Order {request.OrderId} cannot be cancelled in its current status");
        }

        order.Cancel(request.Reason);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var updatedOrder = await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} cancelled successfully", request.OrderId);
            return _mapper.Map<OrderDto>(updatedOrder);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

/// <summary>
/// Ship order command handler
/// </summary>
public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ShipOrderCommandHandler> _logger;

    public ShipOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ShipOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Shipping order {OrderId}", request.OrderId);

        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", request.OrderId);
            return null;
        }

        order.Ship(request.TrackingNumber);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var updatedOrder = await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} shipped successfully", request.OrderId);
            return _mapper.Map<OrderDto>(updatedOrder);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

/// <summary>
/// Deliver order command handler
/// </summary>
public class DeliverOrderCommandHandler : IRequestHandler<DeliverOrderCommand, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DeliverOrderCommandHandler> _logger;

    public DeliverOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<DeliverOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delivering order {OrderId}", request.OrderId);

        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", request.OrderId);
            return null;
        }

        order.Deliver();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var updatedOrder = await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} delivered successfully", request.OrderId);
            return _mapper.Map<OrderDto>(updatedOrder);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
