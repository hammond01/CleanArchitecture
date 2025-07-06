using AutoFixture;
using FluentAssertions;
using ProductManager.Application.Feature.Category.Commands;
using ProductManager.Application.Feature.Category.Queries;
using ProductManager.Domain.Entities;
using Xunit;

namespace ProductManager.UnitTests.Application.Category;

public class CategoryCommandQueryTests
{
    private readonly Fixture _fixture;

    public CategoryCommandQueryTests()
    {
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void AddOrUpdateCategoryCommand_WhenCreated_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var category = _fixture.Create<Categories>();

        // Act
        var command = new AddOrUpdateCategoryCommand(category);

        // Assert
        command.Categories.Should().BeEquivalentTo(category);
        command.Categories.Should().BeSameAs(category);
    }

    [Fact]
    public void AddOrUpdateCategoryCommand_WhenCategoryIsNull_ShouldAllowNull()
    {
        // Act
        var command = new AddOrUpdateCategoryCommand(null!);

        // Assert
        command.Categories.Should().BeNull();
    }

    [Fact]
    public void DeleteCategoryCommand_WhenCreated_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var category = _fixture.Create<Categories>();

        // Act
        var command = new DeleteCategoryCommand(category);

        // Assert
        command.Category.Should().BeEquivalentTo(category);
        command.Category.Should().BeSameAs(category);
    }

    [Fact]
    public void DeleteCategoryCommand_WhenCategoryIsNull_ShouldAllowNull()
    {
        // Act
        var command = new DeleteCategoryCommand(null!);

        // Assert
        command.Category.Should().BeNull();
    }

    [Fact]
    public void GetCategoryByIdQuery_WhenCreated_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var categoryId = "test-category-id";

        // Act
        var query = new GetCategoryByIdQuery(categoryId);

        // Assert
        query.CategoryId.Should().Be(categoryId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void GetCategoryByIdQuery_WhenCategoryIdIsInvalid_ShouldAllowInvalidValues(string? invalidId)
    {
        // Act & Assert
        var query = new GetCategoryByIdQuery(invalidId ?? string.Empty);
        query.CategoryId.Should().Be(invalidId ?? string.Empty);
    }

    [Fact]
    public void GetCategories_WhenCreated_ShouldBeRecord()
    {
        // Act
        var query = new GetCategories();

        // Assert
        query.Should().NotBeNull();
        query.GetType().Should().BeDecoratedWith<System.Runtime.CompilerServices.CompilerGeneratedAttribute>();
    }

    [Fact]
    public void GetCategories_WhenTwoInstancesCreated_ShouldBeEqual()
    {
        // Act
        var query1 = new GetCategories();
        var query2 = new GetCategories();

        // Assert
        query1.Should().Be(query2);
        query1.GetHashCode().Should().Be(query2.GetHashCode());
    }

    [Fact]
    public void AddOrUpdateCategoryCommand_Categories_ShouldBeSettable()
    {
        // Arrange
        var originalCategory = _fixture.Create<Categories>();
        var newCategory = _fixture.Create<Categories>();
        var command = new AddOrUpdateCategoryCommand(originalCategory);

        // Act
        command.Categories = newCategory;

        // Assert
        command.Categories.Should().BeEquivalentTo(newCategory);
        command.Categories.Should().BeSameAs(newCategory);
    }

    [Fact]
    public void DeleteCategoryCommand_Category_ShouldBeSettable()
    {
        // Arrange
        var originalCategory = _fixture.Create<Categories>();
        var newCategory = _fixture.Create<Categories>();
        var command = new DeleteCategoryCommand(originalCategory);

        // Act
        command.Category = newCategory;

        // Assert
        command.Category.Should().BeEquivalentTo(newCategory);
        command.Category.Should().BeSameAs(newCategory);
    }

    [Fact]
    public void GetCategoryByIdQuery_CategoryId_ShouldBeSettable()
    {
        // Arrange
        var originalId = "original-id";
        var newId = "new-id";
        var query = new GetCategoryByIdQuery(originalId);

        // Act
        query.CategoryId = newId;

        // Assert
        query.CategoryId.Should().Be(newId);
    }
}
