using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Feature.Product.Commands;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Entities;
using Xunit;

namespace ProductManager.UnitTests.Application.Product;

public class DeleteProductHandlerTests
{
    private readonly Mock<ICrudService<Products>> _crudServiceMock;
    private readonly DeleteProductCommandHandler _handler;
    private readonly Fixture _fixture;

    public DeleteProductHandlerTests()
    {
        _crudServiceMock = new Mock<ICrudService<Products>>();
        _handler = new DeleteProductCommandHandler(_crudServiceMock.Object);
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task HandleAsync_WithValidProduct_ShouldDeleteProduct()
    {
        // Arrange
        var product = _fixture.Create<Products>();
        var command = new DeleteProductCommand(product);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Message.Should().Be(CRUDMessage.DeleteSuccess);
        result.StatusCode.Should().Be(0); // Default ApiResponse status code when not specified

        _crudServiceMock.Verify(x => x.DeleteAsync(It.Is<Products>(p => p.Id == product.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNullProduct_ShouldStillCallDelete()
    {
        // Arrange
        var command = new DeleteProductCommand(null!);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Message.Should().Be(CRUDMessage.DeleteSuccess);

        _crudServiceMock.Verify(x => x.DeleteAsync(null!, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCrudServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var product = _fixture.Create<Products>();
        var command = new DeleteProductCommand(product);

        _crudServiceMock.Setup(x => x.DeleteAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command, CancellationToken.None));

        _crudServiceMock.Verify(x => x.DeleteAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_ShouldPassTokenToCrudService()
    {
        // Arrange
        var product = _fixture.Create<Products>();
        var command = new DeleteProductCommand(product);
        var cancellationToken = new CancellationToken();

        // Act
        await _handler.HandleAsync(command, cancellationToken);

        // Assert
        _crudServiceMock.Verify(x => x.DeleteAsync(It.IsAny<Products>(), cancellationToken), Times.Once);
    }

    [Fact]
    public void Constructor_WithNullCrudService_ShouldAllowNullButWillFailOnUsage()
    {
        // Act - Constructor allows null (no validation in constructor)
        var handler = new DeleteProductCommandHandler(null!);

        // Assert - Handler is created but will fail when used
        handler.Should().NotBeNull();
    }
}
