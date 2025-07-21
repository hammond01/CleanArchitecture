using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Mappings;
using Shared.Common.Mediator;

namespace ProductCatalog.Application.Products.Queries;

/// <summary>
/// Get all products query
/// </summary>
public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
{
}

/// <summary>
/// Get all products query handler
/// </summary>
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
        return products.ToDto();
    }
}

/// <summary>
/// Get product by ID query
/// </summary>
public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public string ProductId { get; set; } = string.Empty;
}

/// <summary>
/// Get product by ID query handler
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        return product?.ToDto();
    }
}

/// <summary>
/// Get products by category query
/// </summary>
public class GetProductsByCategoryQuery : IRequest<IEnumerable<ProductDto>>
{
    public string CategoryId { get; set; } = string.Empty;
}

/// <summary>
/// Get products by category query handler
/// </summary>
public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsByCategoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetByCategoryAsync(request.CategoryId, cancellationToken);
        return products.ToDto();
    }
}

/// <summary>
/// Get products by supplier query
/// </summary>
public class GetProductsBySupplierQuery : IRequest<IEnumerable<ProductDto>>
{
    public string SupplierId { get; set; } = string.Empty;
}

/// <summary>
/// Get products by supplier query handler
/// </summary>
public class GetProductsBySupplierQueryHandler : IRequestHandler<GetProductsBySupplierQuery, IEnumerable<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsBySupplierQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsBySupplierQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetBySupplierAsync(request.SupplierId, cancellationToken);
        return products.ToDto();
    }
}
