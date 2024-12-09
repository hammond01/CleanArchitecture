namespace ProductManager.Application.Feature.Product.Commands;

public class AddOrUpdateProductCommand : ICommand<ApiResponse>
{
    public AddOrUpdateProductCommand(Products products)
    {
        Products = products;
    }
    public Products Products { get; set; }
}
internal class AddOrUpdateProductHandler : ICommandHandler<AddOrUpdateProductCommand, ApiResponse>
{
    private readonly ICrudService<Products> _crudService;
    private readonly IUnitOfWork _unitOfWork;
    public AddOrUpdateProductHandler(IUnitOfWork unitOfWork, ICrudService<Products> crudService)
    {
        _unitOfWork = unitOfWork;
        _crudService = crudService;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            if (command.Products.Id == null!)
            {
                command.Products.Id = UlidExtension.Generate();
                await _crudService.AddAsync(command.Products, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, CRUDMessage.CreateSuccess);
            }
            await _crudService.UpdateAsync(command.Products, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.UpdateSuccess);
        }
    }
}
