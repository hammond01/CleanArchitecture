using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Domain.Repositories;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Commands;

/// <summary>
/// Handler for CreateOrUpdateCategoryCommand
/// </summary>
public class CreateOrUpdateCategoryCommandHandler : ICommandHandler<CreateOrUpdateCategoryCommand, string>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrUpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> HandleAsync(CreateOrUpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        Category category;

        if (string.IsNullOrEmpty(command.Id))
        {
            // Create new category
            category = new Category
            {
                Id = Guid.NewGuid().ToString(),
                CategoryName = command.CategoryName,
                Description = command.Description,
                PictureLink = command.PictureLink
            };

            await _categoryRepository.AddAsync(category, cancellationToken);
        }
        else
        {
            // Update existing category
            var existingCategory = await _categoryRepository
                .GetQueryableSet()
                .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {command.Id} not found");
            }

            existingCategory.CategoryName = command.CategoryName;
            existingCategory.Description = command.Description;
            existingCategory.PictureLink = command.PictureLink;

            await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);
            category = existingCategory;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
