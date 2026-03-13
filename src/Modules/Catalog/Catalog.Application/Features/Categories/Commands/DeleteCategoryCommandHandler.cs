using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Domain.Repositories;
using Catalog.Domain.Events;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Commands;

/// <summary>
/// Handler for DeleteCategoryCommand
/// </summary>
public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository
            .GetQueryableSet()
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (category == null)
        {
            return false;
        }

        var hasProducts = await _productRepository
            .GetQueryableSet()
            .AnyAsync(p => p.CategoryId == command.Id, cancellationToken);

        if (hasProducts)
        {
            throw new InvalidOperationException("Cannot delete a category that still contains products");
        }

        category.AddDomainEvent(new CategoryDeletedEvent(category.Id, category.CategoryName));
        _categoryRepository.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
