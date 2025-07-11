using Shared.Common.Mediator;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Products.Queries;

/// <summary>
/// Get product by ID query
/// </summary>
public class GetProductByIdQuery : IRequest<Product?>
{
    public string ProductId { get; set; } = string.Empty;

    public GetProductByIdQuery(string productId)
    {
        ProductId = productId;
    }
}

/// <summary>
/// Get product by ID query handler
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
    }
}
