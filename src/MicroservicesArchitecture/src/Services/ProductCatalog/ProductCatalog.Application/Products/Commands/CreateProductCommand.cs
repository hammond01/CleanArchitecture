using Shared.Common.Mediator;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Products.Commands;

/// <summary>
/// Create product command
/// </summary>
public class CreateProductCommand : IRequest<string>
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
/// Create product command handler
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
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

        // Check if product already exists
        var existingProduct = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (existingProduct != null)
            throw new ArgumentException($"Product with ID {request.ProductId} already exists");

        var product = new Product(
            request.ProductId,
            request.ProductName,
            request.SupplierId,
            request.CategoryId,
            request.QuantityPerUnit,
            request.UnitPrice,
            request.UnitsInStock,
            request.UnitsOnOrder,
            request.ReorderLevel,
            request.Discontinued);

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.ProductId;
    }
}
