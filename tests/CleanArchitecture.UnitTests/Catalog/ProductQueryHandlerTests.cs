using Catalog.Application.DTOs;
using Catalog.Application.Features.Products.Queries;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Repositories;
using FluentAssertions;

namespace CleanArchitecture.UnitTests.Catalog;

/// <summary>
/// Unit tests for Product Query Handlers
/// Demonstrates testing CQRS query handlers in isolation
/// </summary>
public class ProductQueryHandlerTests : IClassFixture<CatalogDbContextFixture>
{
    private readonly CatalogDbContextFixture _fixture;

    public ProductQueryHandlerTests(CatalogDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsProductDto()
    {
        // Arrange
        var productId = Guid.NewGuid().ToString();
        var category = Category.Create("Electronics", "Electronic devices");
        var product = Product.Create(
            productName: "Laptop",
            categoryId: category.Id,
            quantityPerUnit: "1 unit",
            unitPrice: 999.99m,
            unitsInStock: 10
        );

        // Use reflection to set the Id (since it's readonly after creation)
        var idProperty = typeof(Product).BaseType!.GetProperty("Id")!;
        idProperty.SetValue(product, productId);

        await using var context = _fixture.CreateDbContext();
        await context.Categories.AddAsync(category);
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        IProductRepository productRepository = new ProductRepository(context);

        var query = new GetProductByIdQuery(productId);
        var handler = new GetProductByIdQueryHandler(productRepository);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.ProductName.Should().Be("Laptop");
        result.UnitPrice.Should().Be(999.99m);
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid().ToString();

        await using var context = _fixture.CreateDbContext();
        IProductRepository productRepository = new ProductRepository(context);

        var query = new GetProductByIdQuery(invalidId);
        var handler = new GetProductByIdQueryHandler(productRepository);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().BeNull();
    }

    // TODO: Add more unit tests for:
    // - GetProductsQuery (with pagination)
    // - GetProductsByCategoryQuery
    // - SearchProductsQuery
    // - Product specification tests (LowStockProducts, ProductsByPrice)
    // - Product command handlers (Create, Update, Delete with validation)
    // - Category query/command handlers
}
