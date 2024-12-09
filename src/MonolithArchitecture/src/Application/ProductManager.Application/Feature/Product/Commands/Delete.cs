namespace ProductManager.Application.Feature.Product.Commands;

public class DeleteProductCommand : ICommand<ApiResponse>
{
    public DeleteProductCommand(Products product)
    {
        Product = product;
    }
    public Products Product { get; set; }
}
public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, ApiResponse>
{
    private readonly ICrudService<Products> _crudService;

    public DeleteProductCommandHandler(ICrudService<Products> crudService)
    {
        _crudService = crudService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await _crudService.DeleteAsync(request.Product, cancellationToken);

        return new ApiResponse
        {
            Message = CRUDMessage.DeleteSuccess
        };
    }
}
