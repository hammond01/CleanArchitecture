using ProductManager.Application.Common.Services;
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
            Message = "Category deleted successfully"
        };
    }
}
