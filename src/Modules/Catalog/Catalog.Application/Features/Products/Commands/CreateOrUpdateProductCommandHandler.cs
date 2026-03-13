using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Domain.Repositories;
using Catalog.Domain.Entities;
using Catalog.Domain.Events;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Commands;

/// <summary>
/// Handler for CreateOrUpdateProductCommand - Uses domain methods for business logic
/// </summary>
public class CreateOrUpdateProductCommandHandler : ICommandHandler<CreateOrUpdateProductCommand, string>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrUpdateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> HandleAsync(CreateOrUpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        Product product;

        if (string.IsNullOrEmpty(command.Id))
        {
            // Create new product using factory method
            product = Product.Create(
                command.ProductName,
                command.CategoryId,
                command.QuantityPerUnit,
                command.UnitPrice,
                command.UnitsInStock,
                command.UnitsOnOrder,
                command.ReorderLevel,
                command.Discontinued);

            product.AddDomainEvent(new ProductCreatedEvent(
                product.Id,
                product.ProductName,
                product.CategoryId,
                product.UnitPrice));
            await _productRepository.AddAsync(product, cancellationToken);
        }
        else
        {
            // Update existing product using domain methods
            var existingProduct = await _productRepository
                .GetQueryableSet()
                .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {command.Id} not found");
            }

            // Use domain methods to update
            existingProduct.UpdateDetails(
                command.ProductName,
                command.QuantityPerUnit,
                command.UnitPrice,
                command.ReorderLevel);

            if (command.UnitsInStock.HasValue || command.UnitsOnOrder.HasValue)
            {
                existingProduct.UpdateStock(
                    command.UnitsInStock ?? existingProduct.UnitsInStock ?? 0,
                    command.UnitsOnOrder ?? existingProduct.UnitsOnOrder ?? 0);
            }

            if (command.Discontinued)
            {
                existingProduct.Discontinue();
            }
            else
            {
                existingProduct.Reactivate();
            }

            existingProduct.AddDomainEvent(new ProductUpdatedEvent(
                existingProduct.Id,
                existingProduct.ProductName,
                existingProduct.UnitPrice));
            await _productRepository.UpdateAsync(existingProduct, cancellationToken);
            product = existingProduct;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
