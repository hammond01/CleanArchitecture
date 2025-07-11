using Shared.Common.Mediator;
using ProductCatalog.Application.Common.Interfaces;

namespace ProductCatalog.Application.Products.Commands;

/// <summary>
/// Update product command
/// </summary>
public class UpdateProductCommand : IRequest<bool>
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? SupplierId { get; set; }
    public string? CategoryId { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
}

/// <summary>
/// Update product command handler
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new ArgumentException($"Product with ID {request.ProductId} not found");

        // Validate if category exists
        if (!string.IsNullOrEmpty(request.CategoryId))
        {
            var categoryExists = await _unitOfWork.Categories.ExistsAsync(request.CategoryId, cancellationToken);
            if (!categoryExists)
                throw new ArgumentException($"Category with ID {request.CategoryId} does not exist");
        }

        // Validate if supplier exists
        if (!string.IsNullOrEmpty(request.SupplierId))
        {
            var supplierExists = await _unitOfWork.Suppliers.ExistsAsync(request.SupplierId, cancellationToken);
            if (!supplierExists)
                throw new ArgumentException($"Supplier with ID {request.SupplierId} does not exist");
        }

        product.UpdateProductInfo(
            request.ProductName,
            request.SupplierId,
            request.CategoryId,
            request.QuantityPerUnit,
            request.UnitPrice,
            request.UnitsInStock,
            request.UnitsOnOrder,
            request.ReorderLevel,
            request.Discontinued);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
