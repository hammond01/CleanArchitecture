using BuildingBlocks.Domain.Repositories;
using Catalog.Application.Features.Categories.Commands;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Repositories;
using FluentAssertions;

namespace CleanArchitecture.UnitTests.Catalog;

public class CategoryCommandHandlerTests : IClassFixture<CatalogDbContextFixture>
{
    private readonly CatalogDbContextFixture _fixture;

    public CategoryCommandHandlerTests(CatalogDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HandleAsync_Create_ReturnsNewId()
    {
        await using var context = _fixture.CreateDbContext();
        ICategoryRepository repository = new CategoryRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new CreateOrUpdateCategoryCommandHandler(repository, unitOfWork);
        var command = new CreateOrUpdateCategoryCommand
        {
            CategoryName = "Beverages",
            Description = "Cold and hot drinks",
            PictureLink = "https://example.com/beverages.png"
        };

        var result = await handler.HandleAsync(command);

        result.Should().NotBeNullOrWhiteSpace();
        var saved = await context.Categories.FindAsync(result);
        saved.Should().NotBeNull();
        saved!.CategoryName.Should().Be("Beverages");
        saved.PictureLink.Should().Be("https://example.com/beverages.png");
    }

    [Fact]
    public async Task HandleAsync_Update_ChangesCategoryDetails()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Old Name", "Old desc");
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        ICategoryRepository repository = new CategoryRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new CreateOrUpdateCategoryCommandHandler(repository, unitOfWork);
        var command = new CreateOrUpdateCategoryCommand
        {
            Id = category.Id,
            CategoryName = "New Name",
            Description = "New desc",
            PictureLink = "https://example.com/new.png"
        };

        var result = await handler.HandleAsync(command);

        result.Should().Be(category.Id);
        var updated = await context.Categories.FindAsync(category.Id);
        updated!.CategoryName.Should().Be("New Name");
        updated.Description.Should().Be("New desc");
        updated.PictureLink.Should().Be("https://example.com/new.png");
    }

    [Fact]
    public async Task HandleAsync_Update_WithMissingCategory_Throws()
    {
        await using var context = _fixture.CreateDbContext();
        ICategoryRepository repository = new CategoryRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new CreateOrUpdateCategoryCommandHandler(repository, unitOfWork);
        var command = new CreateOrUpdateCategoryCommand
        {
            Id = Guid.NewGuid().ToString(),
            CategoryName = "Missing",
            Description = "Missing"
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_Delete_ReturnsTrueWhenRemoved()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Delete Me");
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        ICategoryRepository repository = new CategoryRepository(context);
        IProductRepository productRepository = new ProductRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new DeleteCategoryCommandHandler(repository, productRepository, unitOfWork);
        var command = new DeleteCategoryCommand(category.Id);

        var result = await handler.HandleAsync(command);

        result.Should().BeTrue();
        (await context.Categories.FindAsync(category.Id)).Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_Delete_WithMissingCategory_ReturnsFalse()
    {
        await using var context = _fixture.CreateDbContext();
        ICategoryRepository repository = new CategoryRepository(context);
        IProductRepository productRepository = new ProductRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new DeleteCategoryCommandHandler(repository, productRepository, unitOfWork);
        var command = new DeleteCategoryCommand(Guid.NewGuid().ToString());

        var result = await handler.HandleAsync(command);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_Delete_WithExistingProducts_ThrowsInvalidOperationException()
    {
        await using var context = _fixture.CreateDbContext();
        var category = Category.Create("Protected");
        var product = Product.Create("Attached Product", category.Id);

        await context.Categories.AddAsync(category);
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        ICategoryRepository repository = new CategoryRepository(context);
        IProductRepository productRepository = new ProductRepository(context);
        IUnitOfWork unitOfWork = context;

        var handler = new DeleteCategoryCommandHandler(repository, productRepository, unitOfWork);
        var command = new DeleteCategoryCommand(category.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(command));
    }
}
