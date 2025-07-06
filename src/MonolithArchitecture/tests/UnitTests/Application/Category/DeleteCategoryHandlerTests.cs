using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Feature.Category.Commands;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Entities;
using Xunit;

namespace ProductManager.UnitTests.Application.Category;

public class DeleteCategoryHandlerTests
{
    private readonly Mock<ICrudService<Categories>> _crudServiceMock;
    private readonly DeleteCategoryCommandHandler _handler;
    private readonly Fixture _fixture;

    public DeleteCategoryHandlerTests()
    {
        _crudServiceMock = new Mock<ICrudService<Categories>>();
        _handler = new DeleteCategoryCommandHandler(_crudServiceMock.Object);
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task HandleAsync_WhenValidCategory_ShouldDeleteSuccessfully()
    {
        // Arrange
        var category = _fixture.Create<Categories>();
        var command = new DeleteCategoryCommand(category);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Be(CRUDMessage.DeleteSuccess);

        _crudServiceMock.Verify(x => x.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenDeleteAsyncThrows_ShouldPropagateException()
    {
        // Arrange
        var category = _fixture.Create<Categories>();
        var command = new DeleteCategoryCommand(category);
        var exception = new InvalidOperationException("Database error");

        _crudServiceMock.Setup(x => x.DeleteAsync(It.IsAny<Categories>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act & Assert
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error");

        _crudServiceMock.Verify(x => x.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCancellationRequested_ShouldRespectCancellation()
    {
        // Arrange
        var category = _fixture.Create<Categories>();
        var command = new DeleteCategoryCommand(category);
        var cancellationToken = new CancellationToken(true);

        _crudServiceMock.Setup(x => x.DeleteAsync(It.IsAny<Categories>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        var act = async () => await _handler.HandleAsync(command, cancellationToken);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public void Constructor_WhenCrudServiceIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new DeleteCategoryCommandHandler(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
