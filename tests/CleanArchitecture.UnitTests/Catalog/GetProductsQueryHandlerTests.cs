using Catalog.Application.Features.Products.Queries;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Repositories;
using FluentAssertions;

namespace CleanArchitecture.UnitTests.Catalog;

public class GetProductsQueryHandlerTests : IClassFixture<CatalogDbContextFixture>
{
    private readonly CatalogDbContextFixture _fixture;

    public GetProductsQueryHandlerTests(CatalogDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HandleAsync_WithPagination_ReturnsOrderedPage()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Electronics");
        var productA = Product.Create("A-Item", category.Id, unitPrice: 10m);
        var productB = Product.Create("B-Item", category.Id, unitPrice: 20m);
        var productC = Product.Create("C-Item", category.Id, unitPrice: 30m);

        await context.Categories.AddAsync(category);
        await context.Products.AddRangeAsync(productC, productA, productB);
        await context.SaveChangesAsync();

        IProductRepository productRepository = new ProductRepository(context);
        var handler = new GetProductsQueryHandler(productRepository);

        var query = new GetProductsQuery { PageNumber = 1, PageSize = 2 };

        var result = await handler.HandleAsync(query);

        result.Should().HaveCount(2);
        result[0].ProductName.Should().Be("A-Item");
        result[1].ProductName.Should().Be("B-Item");
        result.All(p => p.CategoryName == category.CategoryName).Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WithCategoryFilter_ReturnsMatchingProducts()
    {
        await using var context = _fixture.CreateDbContext();
        var categoryA = Category.Create("A");
        var categoryB = Category.Create("B");

        var productA1 = Product.Create("Alpha", categoryA.Id);
        var productA2 = Product.Create("Beta", categoryA.Id);
        var productB1 = Product.Create("Gamma", categoryB.Id);

        await context.Categories.AddRangeAsync(categoryA, categoryB);
        await context.Products.AddRangeAsync(productA1, productA2, productB1);
        await context.SaveChangesAsync();

        IProductRepository productRepository = new ProductRepository(context);
        var handler = new GetProductsQueryHandler(productRepository);

        var query = new GetProductsQuery { CategoryId = categoryA.Id };

        var result = await handler.HandleAsync(query);

        result.Should().HaveCount(2);
        result.All(p => p.CategoryId == categoryA.Id).Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WithDiscontinuedFilter_ReturnsOnlyDiscontinued()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Food");
        var activeProduct = Product.Create("Bread", category.Id, discontinued: false);
        var discontinuedProduct = Product.Create("Milk", category.Id, discontinued: true);

        await context.Categories.AddAsync(category);
        await context.Products.AddRangeAsync(activeProduct, discontinuedProduct);
        await context.SaveChangesAsync();

        IProductRepository productRepository = new ProductRepository(context);
        var handler = new GetProductsQueryHandler(productRepository);

        var query = new GetProductsQuery { Discontinued = true };

        var result = await handler.HandleAsync(query);

        result.Should().HaveCount(1);
        result[0].ProductName.Should().Be("Milk");
        result[0].Discontinued.Should().BeTrue();
    }
}
