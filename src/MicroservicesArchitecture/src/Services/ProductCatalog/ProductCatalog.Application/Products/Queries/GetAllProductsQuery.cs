using Shared.Common.Mediator;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Products.Queries;

/// <summary>
/// Get all products query
/// </summary>
public class GetAllProductsQuery : IRequest<IEnumerable<Product>>
{
}

/// <summary>
/// Get all products query handler
/// </summary>
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Products.GetAllAsync(cancellationToken);
    }
}

/// <summary>
/// Get products paged query
/// </summary>
public class GetProductsPagedQuery : IRequest<IEnumerable<Product>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetProductsPagedQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

/// <summary>
/// Get products paged query handler
/// </summary>
public class GetProductsPagedQueryHandler : IRequestHandler<GetProductsPagedQuery, IEnumerable<Product>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Product>> Handle(GetProductsPagedQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Products.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
