using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Mappings;
using Shared.Common.Mediator;

namespace ProductCatalog.Application.Products.Commands;

/// <summary>
/// Update product command
/// </summary>
public class UpdateProductCommand : IRequest<bool>
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? CategoryId { get; set; }
    public string? SupplierId { get; set; }
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
            return false;

        // Create DTO and update entity
        var updateDto = new UpdateProductDto
        {
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId,
            QuantityPerUnit = request.QuantityPerUnit,
            UnitPrice = request.UnitPrice,
            UnitsInStock = request.UnitsInStock,
            UnitsOnOrder = request.UnitsOnOrder,
            ReorderLevel = request.ReorderLevel,
            Discontinued = request.Discontinued
        };

        product.UpdateFromDto(updateDto);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

/// <summary>
/// Delete product command
/// </summary>
public class DeleteProductCommand : IRequest<bool>
{
    public string ProductId { get; set; } = string.Empty;
}

/// <summary>
/// Delete product command handler
/// </summary>
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.Products.ExistsAsync(request.ProductId, cancellationToken);
        if (!exists)
            return false;

        await _unitOfWork.Products.DeleteAsync(request.ProductId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
