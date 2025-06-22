using System.Data;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Persistence.Extensions;
namespace ProductManager.Application.Feature.Category.Commands;

public class AddOrUpdateCategoryCommand : ICommand<ApiResponse>
{
    public AddOrUpdateCategoryCommand(Categories categories)
    {
        Categories = categories;
    }
    public Categories Categories { get; set; }
}
internal class AddOrUpdateCategoryHandler : ICommandHandler<AddOrUpdateCategoryCommand, ApiResponse>
{
    private readonly ICrudService<Categories> _categoryService;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrUpdateCategoryHandler(ICrudService<Categories> categoryService, IUnitOfWork unitOfWork)
    {
        _categoryService = categoryService;
        _unitOfWork = unitOfWork;
    }
    public async Task<ApiResponse> HandleAsync(AddOrUpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            if (command.Categories.Id == null!)
            {
                command.Categories.Id = UlidExtension.Generate();
                await _categoryService.AddAsync(command.Categories, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, CRUDMessage.CreateSuccess);
            }
            await _categoryService.UpdateAsync(command.Categories, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.UpdateSuccess);
        }
    }
}
