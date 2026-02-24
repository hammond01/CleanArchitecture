using Catalog.Application.Features.Categories.Queries;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Repositories;
using FluentAssertions;

namespace CleanArchitecture.UnitTests.Catalog;

public class GetCategoryByIdQueryHandlerTests : IClassFixture<CatalogDbContextFixture>
{
    private readonly CatalogDbContextFixture _fixture;

    public GetCategoryByIdQueryHandlerTests(CatalogDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HandleAsync_WithValidId_ReturnsCategory()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Seafood", description: "Fresh fish");

        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        ICategoryRepository repository = new CategoryRepository(context);
        var handler = new GetCategoryByIdQueryHandler(repository);
        var query = new GetCategoryByIdQuery(category.Id);

        var result = await handler.HandleAsync(query);

        result.Should().NotBeNull();
        result!.Id.Should().Be(category.Id);
        result.CategoryName.Should().Be("Seafood");
    }

    [Fact]
    public async Task HandleAsync_WithInvalidId_ReturnsNull()
    {
        await using var context = _fixture.CreateDbContext();
        ICategoryRepository repository = new CategoryRepository(context);
        var handler = new GetCategoryByIdQueryHandler(repository);
        var query = new GetCategoryByIdQuery(Guid.NewGuid().ToString());

        var result = await handler.HandleAsync(query);

        result.Should().BeNull();
    }
}
