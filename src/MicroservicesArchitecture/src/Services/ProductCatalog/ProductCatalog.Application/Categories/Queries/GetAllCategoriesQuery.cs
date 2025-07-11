using Shared.Common.Mediator;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Categories.Queries;

/// <summary>
/// Get all categories query
/// </summary>
public class GetAllCategoriesQuery : IRequest<IEnumerable<Category>>
{
}

/// <summary>
/// Get all categories query handler
/// </summary>
public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<Category>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Categories.GetAllAsync(cancellationToken);
    }
}

/// <summary>
/// Get category by ID query
/// </summary>
public class GetCategoryByIdQuery : IRequest<Category?>
{
    public string CategoryId { get; set; } = string.Empty;

    public GetCategoryByIdQuery(string categoryId)
    {
        CategoryId = categoryId;
    }
}

/// <summary>
/// Get category by ID query handler
/// </summary>
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Category?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Category?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
    }
}
