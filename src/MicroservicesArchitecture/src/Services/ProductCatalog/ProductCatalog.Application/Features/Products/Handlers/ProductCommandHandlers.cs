using Shared.Common.Mediator;
using ProductCatalog.Application.Features.Products.Commands;
using ProductCatalog.Application.Common.Models;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Application.Features.Products.Handlers;

/// <summary>
/// Create product command handler
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken = default)
    {
        // Validate category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.CategoryId);

        // Validate supplier exists
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null)
            throw new NotFoundException(nameof(Supplier), request.SupplierId);

        // Check if product name already exists
        var existingProduct = await _unitOfWork.Products.GetByNameAsync(request.ProductName, cancellationToken);
        if (existingProduct != null)
            throw new ValidationException($"Product with name '{request.ProductName}' already exists.");

        // Create new product
        var product = new Product(
            request.ProductName,
            request.CategoryId,
            request.SupplierId,
            request.UnitPrice,
            request.UnitsInStock,
            request.Description);

        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            product.SetImageUrl(request.ImageUrl);

        if (request.Weight > 0)
            product.SetWeight(request.Weight);

        if (!string.IsNullOrWhiteSpace(request.Dimensions))
            product.SetDimensions(request.Dimensions);

        if (request.ReorderLevel >= 0)
            product.SetReorderLevel(request.ReorderLevel);

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}

/// <summary>
/// Update product command handler
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        // Check if new name conflicts with existing products
        var existingProduct = await _unitOfWork.Products.GetByNameAsync(request.ProductName, cancellationToken);
        if (existingProduct != null && existingProduct.Id != request.Id)
            throw new ValidationException($"Product with name '{request.ProductName}' already exists.");

        product.UpdateProductInfo(request.ProductName, request.Description);
        product.UpdatePrice(request.UnitPrice);
        product.SetReorderLevel(request.ReorderLevel);

        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            product.SetImageUrl(request.ImageUrl);

        if (request.Weight > 0)
            product.SetWeight(request.Weight);

        if (!string.IsNullOrWhiteSpace(request.Dimensions))
            product.SetDimensions(request.Dimensions);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

/// <summary>
/// Update product stock command handler
/// </summary>
public class UpdateProductStockCommandHandler : IRequestHandler<UpdateProductStockCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductStockCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        product.UpdateStock(request.NewStock);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

/// <summary>
/// Reduce product stock command handler
/// </summary>
public class ReduceProductStockCommandHandler : IRequestHandler<ReduceProductStockCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReduceProductStockCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(ReduceProductStockCommand request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        product.ReduceStock(request.Quantity);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

/// <summary>
/// Increase product stock command handler
/// </summary>
public class IncreaseProductStockCommandHandler : IRequestHandler<IncreaseProductStockCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public IncreaseProductStockCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(IncreaseProductStockCommand request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        product.IncreaseStock(request.Quantity);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

/// <summary>
/// Discontinue product command handler
/// </summary>
public class DiscontinueProductCommandHandler : IRequestHandler<DiscontinueProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DiscontinueProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(DiscontinueProductCommand request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        product.Discontinue();

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

/// <summary>
/// Activate product command handler
/// </summary>
public class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(ActivateProductCommand request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        product.Activate();

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

/// <summary>
/// Delete product command handler
/// </summary>
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        await _unitOfWork.Products.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
