using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
namespace ProductManager.Application.Feature.Category.Commands;

public class DeleteCategoryCommand : ICommand<ApiResponse>
{
    public DeleteCategoryCommand(Categories category)
    {
        Category = category;
    }
    public Categories Category { get; set; }
}
public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, ApiResponse>
{
    private readonly ICrudService<Categories> _categoryService;

    public DeleteCategoryCommandHandler(ICrudService<Categories> categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(request.Category, cancellationToken);

        return new ApiResponse
        {
            Message = CRUDMessage.DeleteSuccess
        };
    }
}
