using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Queries;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Handlers.Queries;

/// <summary>
/// Get order by ID query handler
/// </summary>
public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;

    public GetOrderByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetOrderByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting order by ID {OrderId}", request.OrderId);

        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", request.OrderId);
            return null;
        }

        return _mapper.Map<OrderDto>(order);
    }
}

/// <summary>
/// Get order by order number query handler
/// </summary>
public class GetOrderByOrderNumberQueryHandler : IRequestHandler<GetOrderByOrderNumberQuery, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOrderByOrderNumberQueryHandler> _logger;

    public GetOrderByOrderNumberQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetOrderByOrderNumberQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(GetOrderByOrderNumberQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting order by order number {OrderNumber}", request.OrderNumber);

        var order = await _unitOfWork.Orders.GetByOrderNumberAsync(request.OrderNumber, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order with number {OrderNumber} not found", request.OrderNumber);
            return null;
        }

        return _mapper.Map<OrderDto>(order);
    }
}

/// <summary>
/// Get orders query handler
/// </summary>
public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResultDto<OrderSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOrdersQueryHandler> _logger;

    public GetOrdersQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetOrdersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<OrderSummaryDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting orders with pagination - Page: {PageNumber}, Size: {PageSize}", 
            request.PageNumber, request.PageSize);

        var (orders, totalCount) = await _unitOfWork.Orders.GetPagedAsync(
            request.PageNumber, 
            request.PageSize, 
            request.Status, 
            cancellationToken);

        var orderDtos = _mapper.Map<IEnumerable<OrderSummaryDto>>(orders);

        return new PagedResultDto<OrderSummaryDto>
        {
            Items = orderDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

/// <summary>
/// Get orders by customer ID query handler
/// </summary>
public class GetOrdersByCustomerIdQueryHandler : IRequestHandler<GetOrdersByCustomerIdQuery, IEnumerable<OrderSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOrdersByCustomerIdQueryHandler> _logger;

    public GetOrdersByCustomerIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetOrdersByCustomerIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderSummaryDto>> Handle(GetOrdersByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting orders for customer {CustomerId}", request.CustomerId);

        var orders = await _unitOfWork.Orders.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return _mapper.Map<IEnumerable<OrderSummaryDto>>(orders);
    }
}

/// <summary>
/// Get orders by status query handler
/// </summary>
public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, IEnumerable<OrderSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOrdersByStatusQueryHandler> _logger;

    public GetOrdersByStatusQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetOrdersByStatusQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderSummaryDto>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting orders with status {Status}", request.Status);

        var orders = await _unitOfWork.Orders.GetByStatusAsync(request.Status, cancellationToken);
        return _mapper.Map<IEnumerable<OrderSummaryDto>>(orders);
    }
}

/// <summary>
/// Get orders by date range query handler
/// </summary>
public class GetOrdersByDateRangeQueryHandler : IRequestHandler<GetOrdersByDateRangeQuery, IEnumerable<OrderSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOrdersByDateRangeQueryHandler> _logger;

    public GetOrdersByDateRangeQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetOrdersByDateRangeQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderSummaryDto>> Handle(GetOrdersByDateRangeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting orders from {StartDate} to {EndDate}", request.StartDate, request.EndDate);

        var orders = await _unitOfWork.Orders.GetByDateRangeAsync(request.StartDate, request.EndDate, cancellationToken);
        return _mapper.Map<IEnumerable<OrderSummaryDto>>(orders);
    }
}

/// <summary>
/// Get order statistics query handler
/// </summary>
public class GetOrderStatisticsQueryHandler : IRequestHandler<GetOrderStatisticsQuery, OrderStatisticsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOrderStatisticsQueryHandler> _logger;

    public GetOrderStatisticsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetOrderStatisticsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderStatisticsDto> Handle(GetOrderStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting order statistics");

        var statistics = await _unitOfWork.Orders.GetStatisticsAsync(request.StartDate, request.EndDate, cancellationToken);
        return _mapper.Map<OrderStatisticsDto>(statistics);
    }
}
