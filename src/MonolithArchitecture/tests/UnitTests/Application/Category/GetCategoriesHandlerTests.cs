using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Feature.Category.Queries;
using ProductManager.Domain.Entities;
using Xunit;

namespace ProductManager.UnitTests.Application.Category;

public class GetCategoriesHandlerTests
{
    private readonly Mock<ICrudService<Categories>> _crudServiceMock;
    private readonly GetsCategoryHandler _handler;
    private readonly Fixture _fixture;

    public GetCategoriesHandlerTests()
    {
        _crudServiceMock = new Mock<ICrudService<Categories>>();
        _handler = new GetsCategoryHandler(_crudServiceMock.Object);
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task HandleAsync_WhenCategoriesExist_ShouldReturnSuccessResponse()
    {
        // Arrange
        var categories = _fixture.CreateMany<Categories>(3).ToList();
        var query = new GetCategories();

        _crudServiceMock.Setup(x => x.GetAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Get categories successfully");
        result.Result.Should().BeEquivalentTo(categories);

        _crudServiceMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenNoCategoriesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyCategories = new List<Categories>();
        var query = new GetCategories();

        _crudServiceMock.Setup(x => x.GetAsync())
            .ReturnsAsync(emptyCategories);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Get categories successfully");
        result.Result.Should().BeEquivalentTo(emptyCategories);

        _crudServiceMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var query = new GetCategories();
        var exception = new InvalidOperationException("Database error");

        _crudServiceMock.Setup(x => x.GetAsync())
            .ThrowsAsync(exception);

        // Act & Assert
        var act = async () => await _handler.HandleAsync(query);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error");

        _crudServiceMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCancellationRequested_ShouldRespectCancellation()
    {
        // Arrange
        var query = new GetCategories();
        var cancellationToken = new CancellationToken(true);

        _crudServiceMock.Setup(x => x.GetAsync())
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        var act = async () => await _handler.HandleAsync(query, cancellationToken);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public void Constructor_WhenCrudServiceIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new GetsCategoryHandler(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
