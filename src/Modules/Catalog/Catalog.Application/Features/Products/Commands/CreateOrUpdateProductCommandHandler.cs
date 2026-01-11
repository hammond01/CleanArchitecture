using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Domain.Repositories;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Commands;

/// <summary>
/// Handler for CreateOrUpdateProductCommand
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
            // Create new product
            product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                ProductName = command.ProductName,
                CategoryId = command.CategoryId,
                QuantityPerUnit = command.QuantityPerUnit,
                UnitPrice = command.UnitPrice,
                UnitsInStock = command.UnitsInStock,
                UnitsOnOrder = command.UnitsOnOrder,
                ReorderLevel = command.ReorderLevel,
                Discontinued = command.Discontinued
            };

            await _productRepository.AddAsync(product, cancellationToken);
        }
        else
        {
            // Update existing product
            var existingProduct = await _productRepository
                .GetQueryableSet()
                .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {command.Id} not found");
            }

            existingProduct.ProductName = command.ProductName;
            existingProduct.CategoryId = command.CategoryId;
            existingProduct.QuantityPerUnit = command.QuantityPerUnit;
            existingProduct.UnitPrice = command.UnitPrice;
            existingProduct.UnitsInStock = command.UnitsInStock;
            existingProduct.UnitsOnOrder = command.UnitsOnOrder;
            existingProduct.ReorderLevel = command.ReorderLevel;
            existingProduct.Discontinued = command.Discontinued;

            await _productRepository.UpdateAsync(existingProduct, cancellationToken);
            product = existingProduct;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
