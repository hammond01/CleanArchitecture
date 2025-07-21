using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Mappings;
using Shared.Common.Mediator;

namespace ProductCatalog.Application.Products.Commands;

/// <summary>
/// Create product command
/// </summary>
public class CreateProductCommand : IRequest<string>
{
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
        // Generate new product ID
        var productId = GenerateProductId();

        // Create DTO and convert to entity
        var createDto = new CreateProductDto
        {
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

        var product = createDto.ToEntity(productId);

        // Add to repository
        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return productId;
    }

    private static string GenerateProductId()
    {
        // Generate ULID-like ID
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var random = Guid.NewGuid().ToString("N")[..10].ToUpper();
        return $"PRD{timestamp:X}{random}";
    }
}
