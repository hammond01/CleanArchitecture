using BuildingBlocks.Domain.Repositories;
using Catalog.Application.Features.Products.Commands;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Repositories;
using FluentAssertions;

namespace CleanArchitecture.UnitTests.Catalog;

public class ProductCommandHandlerTests : IClassFixture<CatalogDbContextFixture>
{
    private readonly CatalogDbContextFixture _fixture;

    public ProductCommandHandlerTests(CatalogDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HandleAsync_Create_ReturnsNewId()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Produce");
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        IProductRepository repository = new ProductRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new CreateOrUpdateProductCommandHandler(repository, unitOfWork);
        var command = new CreateOrUpdateProductCommand
        {
            ProductName = "Apples",
            CategoryId = category.Id,
            QuantityPerUnit = "1 kg",
            UnitPrice = 2.5m,
            UnitsInStock = 10,
            UnitsOnOrder = 2,
            ReorderLevel = 5,
            Discontinued = false
        };

        var result = await handler.HandleAsync(command);

        result.Should().NotBeNullOrWhiteSpace();
        var saved = await context.Products.FindAsync(result);
        saved.Should().NotBeNull();
        saved!.ProductName.Should().Be("Apples");
        saved.UnitPrice.Should().Be(2.5m);
        saved.Discontinued.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_Update_ChangesProductDetails()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Dairy");
        var product = Product.Create(
            productName: "Milk",
            categoryId: category.Id,
            quantityPerUnit: "1L",
            unitPrice: 1.5m,
            unitsInStock: 4,
            unitsOnOrder: 0,
            reorderLevel: 3,
            discontinued: false);

        await context.Categories.AddAsync(category);
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        IProductRepository repository = new ProductRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new CreateOrUpdateProductCommandHandler(repository, unitOfWork);
        var command = new CreateOrUpdateProductCommand
        {
            Id = product.Id,
            ProductName = "Milk 2%",
            CategoryId = category.Id,
            QuantityPerUnit = "2L",
            UnitPrice = 2.75m,
            UnitsInStock = 8,
            UnitsOnOrder = 1,
            ReorderLevel = 4,
            Discontinued = true
        };

        var result = await handler.HandleAsync(command);

        result.Should().Be(product.Id);
        var updated = await context.Products.FindAsync(product.Id);
        updated!.ProductName.Should().Be("Milk 2%");
        updated.QuantityPerUnit.Should().Be("2L");
        updated.UnitPrice.Should().Be(2.75m);
        updated.UnitsInStock.Should().Be(8);
        updated.UnitsOnOrder.Should().Be(1);
        updated.ReorderLevel.Should().Be(4);
        updated.Discontinued.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_Update_WithMissingProduct_Throws()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Frozen");
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        IProductRepository repository = new ProductRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new CreateOrUpdateProductCommandHandler(repository, unitOfWork);
        var command = new CreateOrUpdateProductCommand
        {
            Id = Guid.NewGuid().ToString(),
            ProductName = "Missing",
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_Delete_ReturnsTrueWhenRemoved()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Bakery");
        var product = Product.Create("Bread", category.Id);

        await context.Categories.AddAsync(category);
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        IProductRepository repository = new ProductRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new DeleteProductCommandHandler(repository, unitOfWork);
        var command = new DeleteProductCommand(product.Id);

        var result = await handler.HandleAsync(command);

        result.Should().BeTrue();
        (await context.Products.FindAsync(product.Id)).Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_Delete_WithMissingProduct_ReturnsFalse()
    {
        await using var context = _fixture.CreateDbContext();
        IProductRepository repository = new ProductRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new DeleteProductCommandHandler(repository, unitOfWork);
        var command = new DeleteProductCommand(Guid.NewGuid().ToString());

        var result = await handler.HandleAsync(command);

        result.Should().BeFalse();
    }
}
