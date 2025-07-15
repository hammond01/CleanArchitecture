using Shared.Common.Mediator;
using ProductCatalog.Application.Features.Products.Queries;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Common.Models;
using ProductCatalog.Domain.Repositories;
using AutoMapper;

namespace ProductCatalog.Application.Features.Products.Handlers;

/// <summary>
/// Get product by ID query handler
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }
}

/// <summary>
/// Get all products query handler
/// </summary>
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductSummaryDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProductSummaryDto>>(products);
    }
}

/// <summary>
/// Get products by category query handler
/// </summary>
public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductsByCategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductSummaryDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetByCategoryIdAsync(request.CategoryId, cancellationToken);
        return _mapper.Map<IEnumerable<ProductSummaryDto>>(products);
    }
}

/// <summary>
/// Get products by supplier query handler
/// </summary>
public class GetProductsBySupplierQueryHandler : IRequestHandler<GetProductsBySupplierQuery, IEnumerable<ProductSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductsBySupplierQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductSummaryDto>> Handle(GetProductsBySupplierQuery request, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetBySupplierIdAsync(request.SupplierId, cancellationToken);
        return _mapper.Map<IEnumerable<ProductSummaryDto>>(products);
    }
}

/// <summary>
/// Get active products query handler
/// </summary>
public class GetActiveProductsQueryHandler : IRequestHandler<GetActiveProductsQuery, IEnumerable<ProductSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetActiveProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductSummaryDto>> Handle(GetActiveProductsQuery request, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetActiveProductsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProductSummaryDto>>(products);
    }
}

/// <summary>
/// Get low stock products query handler
/// </summary>
public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, IEnumerable<ProductSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLowStockProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductSummaryDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetLowStockProductsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProductSummaryDto>>(products);
    }
}

/// <summary>
/// Get out of stock products query handler
/// </summary>
public class GetOutOfStockProductsQueryHandler : IRequestHandler<GetOutOfStockProductsQuery, IEnumerable<ProductSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOutOfStockProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductSummaryDto>> Handle(GetOutOfStockProductsQuery request, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetOutOfStockProductsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProductSummaryDto>>(products);
    }
}

/// <summary>
/// Search products query handler
/// </summary>
public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, IEnumerable<ProductSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SearchProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductSummaryDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.SearchByNameAsync(request.SearchTerm, cancellationToken);
        return _mapper.Map<IEnumerable<ProductSummaryDto>>(products);
    }
}

/// <summary>
/// Get products by price range query handler
/// </summary>
public class GetProductsByPriceRangeQueryHandler : IRequestHandler<GetProductsByPriceRangeQuery, IEnumerable<ProductSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductsByPriceRangeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductSummaryDto>> Handle(GetProductsByPriceRangeQuery request, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetProductsByPriceRangeAsync(request.MinPrice, request.MaxPrice, cancellationToken);
        return _mapper.Map<IEnumerable<ProductSummaryDto>>(products);
    }
}

/// <summary>
/// Get paged products query handler
/// </summary>
public class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, PagedResponse<ProductSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPagedProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResponse<ProductSummaryDto>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken = default)
    {
        // Get total count first
        var totalCount = await _unitOfWork.Products.GetTotalCountAsync(cancellationToken);

        // Get paged data
        var products = await _unitOfWork.Products.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        // Apply filters if specified (this would be better handled in repository with proper SQL filtering)
        var filteredProducts = products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.CategoryId))
        {
            filteredProducts = filteredProducts.Where(p => p.CategoryId == request.CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(request.SupplierId))
        {
            filteredProducts = filteredProducts.Where(p => p.SupplierId == request.SupplierId);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            filteredProducts = filteredProducts.Where(p =>
                p.ProductName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (request.MinPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.UnitPrice >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.UnitPrice <= request.MaxPrice.Value);
        }

        if (request.ActiveOnly.HasValue && request.ActiveOnly.Value)
        {
            filteredProducts = filteredProducts.Where(p => !p.Discontinued);
        }

        var mappedProducts = _mapper.Map<IEnumerable<ProductSummaryDto>>(filteredProducts);

        return new PagedResponse<ProductSummaryDto>(
            mappedProducts,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}

/// <summary>
/// Check product exists query handler
/// </summary>
public class CheckProductExistsQueryHandler : IRequestHandler<CheckProductExistsQuery, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckProductExistsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(CheckProductExistsQuery request, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Products.ExistsAsync(request.Id, cancellationToken);
    }
}
