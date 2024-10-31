using System.Data;
using ProductManager.Application.Common.Services;
namespace ProductManager.Application.Feature.Category.Commands;

public class CreateCategoryCommand : ICommand<ApiResponse>
{
    public CreateCategoryCommand(CreateCategoryDto createCategoryDto)
    {
        CreateCategoryDto = createCategoryDto;
    }
    public CreateCategoryDto CreateCategoryDto { get; set; }
}
internal class CreateCategoryHandler : ICommandHandler<CreateCategoryCommand, ApiResponse>
{
    private readonly ICrudService<Categories> _categoryService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(ICrudService<Categories> categoryService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _categoryService = categoryService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<ApiResponse> HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            var category = _mapper.Map<Categories>(command.CreateCategoryDto);
            category.Id = Guid.NewGuid().ToString();
            await _categoryService.AddAsync(category, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(201, "Category created successfully");
        }
    }
}
