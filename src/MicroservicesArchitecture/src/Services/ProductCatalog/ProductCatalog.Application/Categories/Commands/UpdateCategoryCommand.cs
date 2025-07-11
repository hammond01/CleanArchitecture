using Shared.Common.Mediator;
using ProductCatalog.Application.Common.Interfaces;

namespace ProductCatalog.Application.Categories.Commands;

/// <summary>
/// Update category command
/// </summary>
public class UpdateCategoryCommand : IRequest<bool>
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public byte[]? Picture { get; set; }
    public string? PictureLink { get; set; }
}

/// <summary>
/// Update category command handler
/// </summary>
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
            throw new ArgumentException($"Category with ID {request.CategoryId} not found");

        category.UpdateCategory(
            request.CategoryName,
            request.Description,
            request.Picture,
            request.PictureLink);

        await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
