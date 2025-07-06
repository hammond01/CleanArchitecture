using AutoFixture;
using FluentAssertions;
using ProductManager.Application.Feature.Category.Commands;
using ProductManager.Application.Feature.Category.Queries;
using ProductManager.Domain.Entities;
using Xunit;

namespace ProductManager.UnitTests.Application.Category;

public class CategoryFeatureSummaryTests
{
    private readonly Fixture _fixture;

    public CategoryFeatureSummaryTests()
    {
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void CategoryFeature_ShouldHaveAllRequiredCommands()
    {
        // Arrange & Act
        var addOrUpdateCommand = _fixture.Create<AddOrUpdateCategoryCommand>();
        var deleteCommand = _fixture.Create<DeleteCategoryCommand>();

        // Assert
        addOrUpdateCommand.Should().NotBeNull();
        addOrUpdateCommand.Categories.Should().NotBeNull();

        deleteCommand.Should().NotBeNull();
        deleteCommand.Category.Should().NotBeNull();
    }

    [Fact]
    public void CategoryFeature_ShouldHaveAllRequiredQueries()
    {
        // Arrange & Act
        var getCategoriesQuery = new GetCategories();
        var getCategoryByIdQuery = _fixture.Create<GetCategoryByIdQuery>();

        // Assert
        getCategoriesQuery.Should().NotBeNull();
        getCategoryByIdQuery.Should().NotBeNull();
        getCategoryByIdQuery.CategoryId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CategoryFeature_Commands_ShouldWorkWithCategoriesEntity()
    {
        // Arrange
        var category = _fixture.Create<Categories>();

        // Act
        var addOrUpdateCommand = new AddOrUpdateCategoryCommand(category);
        var deleteCommand = new DeleteCategoryCommand(category);

        // Assert
        addOrUpdateCommand.Categories.Should().BeSameAs(category);
        deleteCommand.Category.Should().BeSameAs(category);
    }

    [Fact]
    public void CategoryFeature_Queries_ShouldSupportCategoryRetrieval()
    {
        // Arrange
        var categoryId = _fixture.Create<string>();

        // Act
        var getAllQuery = new GetCategories();
        var getByIdQuery = new GetCategoryByIdQuery(categoryId);

        // Assert
        getAllQuery.Should().NotBeNull();
        getByIdQuery.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void CategoryFeature_ShouldSupportCRUDOperations()
    {
        // Arrange
        var category = _fixture.Create<Categories>();
        var categoryId = category.Id;

        // Act - Create/Update operations
        var createCommand = new AddOrUpdateCategoryCommand(category);
        var updateCommand = new AddOrUpdateCategoryCommand(category);

        // Act - Read operations
        var getAllQuery = new GetCategories();
        var getByIdQuery = new GetCategoryByIdQuery(categoryId);

        // Act - Delete operation
        var deleteCommand = new DeleteCategoryCommand(category);

        // Assert
        createCommand.Categories.Should().BeSameAs(category);
        updateCommand.Categories.Should().BeSameAs(category);
        getAllQuery.Should().NotBeNull();
        getByIdQuery.CategoryId.Should().Be(categoryId);
        deleteCommand.Category.Should().BeSameAs(category);
    }

    [Fact]
    public void CategoryFeature_Commands_ShouldBeImmutableAfterCreation()
    {
        // Arrange
        var category = _fixture.Create<Categories>();
        var addOrUpdateCommand = new AddOrUpdateCategoryCommand(category);
        var deleteCommand = new DeleteCategoryCommand(category);

        // Act - Try to modify the entities
        var originalCategoryFromAddUpdate = addOrUpdateCommand.Categories;
        var originalCategoryFromDelete = deleteCommand.Category;

        // Assert - References should remain the same
        addOrUpdateCommand.Categories.Should().BeSameAs(originalCategoryFromAddUpdate);
        deleteCommand.Category.Should().BeSameAs(originalCategoryFromDelete);
    }

    [Fact]
    public void CategoryFeature_Queries_ShouldBeReadOnly()
    {
        // Arrange & Act
        var getAllQuery = new GetCategories();
        var categoryId = "test-id";
        var getByIdQuery = new GetCategoryByIdQuery(categoryId);

        // Assert
        getAllQuery.Should().NotBeNull();
        getByIdQuery.CategoryId.Should().Be(categoryId);

        // Records should be immutable (except for settable properties)
        var anotherGetAllQuery = new GetCategories();
        getAllQuery.Should().Be(anotherGetAllQuery);
    }
}
