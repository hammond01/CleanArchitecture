using Catalog.Application.Features.Categories.Queries;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Repositories;
using FluentAssertions;

namespace CleanArchitecture.UnitTests.Catalog;

public class GetCategoriesQueryHandlerTests : IClassFixture<CatalogDbContextFixture>
{
    private readonly CatalogDbContextFixture _fixture;

    public GetCategoriesQueryHandlerTests(CatalogDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HandleAsync_WithPagination_ReturnsOrderedPage()
    {
        await using var context = _fixture.CreateDbContext();
        var categoryA = Category.Create("Alpha");
        var categoryB = Category.Create("Beta");
        var categoryC = Category.Create("Gamma");

        await context.Categories.AddRangeAsync(categoryC, categoryA, categoryB);
        await context.SaveChangesAsync();

        ICategoryRepository repository = new CategoryRepository(context);
        var handler = new GetCategoriesQueryHandler(repository);
        var query = new GetCategoriesQuery { PageNumber = 1, PageSize = 2 };

        var result = await handler.HandleAsync(query);

        result.Should().HaveCount(2);
        result[0].CategoryName.Should().Be("Alpha");
        result[1].CategoryName.Should().Be("Beta");
    }

    [Fact]
    public async Task HandleAsync_WithSearchTerm_ReturnsMatchingCategories()
    {
        await using var context = _fixture.CreateDbContext();
        var matching = Category.Create("Snacks", description: "Tasty snacks");
        var other = Category.Create("Drinks", description: "Refreshing");

        await context.Categories.AddRangeAsync(matching, other);
        await context.SaveChangesAsync();

        ICategoryRepository repository = new CategoryRepository(context);
        var handler = new GetCategoriesQueryHandler(repository);
        var query = new GetCategoriesQuery { SearchTerm = "snack" };

        var result = await handler.HandleAsync(query);

        result.Should().ContainSingle();
        result[0].CategoryName.Should().Be("Snacks");
    }
}
