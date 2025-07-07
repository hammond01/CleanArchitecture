using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Feature.Category.Queries;
using ProductManager.Domain.Entities;
using Xunit;

namespace ProductManager.UnitTests.Application.Category;

public class GetCategoryByIdHandlerTests
{
    private readonly Mock<ICrudService<Categories>> _crudServiceMock;
    private readonly GetCategoryByIdHandler _handler;
    private readonly Fixture _fixture;

    public GetCategoryByIdHandlerTests()
    {
        _crudServiceMock = new Mock<ICrudService<Categories>>();
        _handler = new GetCategoryByIdHandler(_crudServiceMock.Object);
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task HandleAsync_WhenCategoryExists_ShouldReturnSuccessResponse()
    {
        // Arrange
        var categoryId = "test-category-id";
        var category = _fixture.Build<Categories>()
            .With(c => c.Id, categoryId)
            .Create();
        var query = new GetCategoryByIdQuery(categoryId);

        _crudServiceMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Get data by Id successfully");
        result.Result.Should().BeEquivalentTo(category);

        _crudServiceMock.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCategoryDoesNotExist_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var categoryId = "non-existent-id";
        var query = new GetCategoryByIdQuery(categoryId);

        _crudServiceMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Categories)null!);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("No data found.");
        result.Result.Should().BeNull();

        _crudServiceMock.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var categoryId = "test-category-id";
        var query = new GetCategoryByIdQuery(categoryId);
        var exception = new InvalidOperationException("Database error");

        _crudServiceMock.Setup(x => x.GetByIdAsync(categoryId))
            .ThrowsAsync(exception);

        // Act & Assert
        var act = async () => await _handler.HandleAsync(query, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error");

        _crudServiceMock.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCancellationRequested_ShouldRespectCancellation()
    {
        // Arrange
        var categoryId = "test-category-id";
        var query = new GetCategoryByIdQuery(categoryId);
        var cancellationToken = new CancellationToken(true);

        _crudServiceMock.Setup(x => x.GetByIdAsync(categoryId))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        var act = async () => await _handler.HandleAsync(query, cancellationToken);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task HandleAsync_WhenCategoryIdIsInvalid_ShouldStillCallService(string? invalidId)
    {
        // Arrange
        var actualId = invalidId ?? string.Empty;
        var query = new GetCategoryByIdQuery(actualId);

        _crudServiceMock.Setup(x => x.GetByIdAsync(actualId))
            .ReturnsAsync((Categories)null!);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);

        _crudServiceMock.Verify(x => x.GetByIdAsync(actualId), Times.Once);
    }

    [Fact]
    public void Constructor_WhenCrudServiceIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new GetCategoryByIdHandler(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
