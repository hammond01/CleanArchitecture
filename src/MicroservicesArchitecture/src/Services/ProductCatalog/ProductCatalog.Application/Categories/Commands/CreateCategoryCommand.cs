using Shared.Common.Mediator;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Categories.Commands;

/// <summary>
/// Create category command
/// </summary>
public class CreateCategoryCommand : IRequest<string>
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public byte[]? Picture { get; set; }
    public string? PictureLink { get; set; }
}

/// <summary>
/// Create category command handler
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Check if category already exists
        var existingCategory = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (existingCategory != null)
            throw new ArgumentException($"Category with ID {request.CategoryId} already exists");

        var category = new Category(
            request.CategoryId,
            request.CategoryName,
            request.Description,
            request.Picture,
            request.PictureLink);

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.CategoryId;
    }
}
