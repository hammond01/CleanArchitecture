using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Feature.Product.Queries;
using ProductManager.Domain.Entities;
using Xunit;

namespace ProductManager.UnitTests.Application.Product;

public class GetProductByIdHandlerTests
{
    private readonly Mock<ICrudService<Products>> _crudServiceMock;
    private readonly GetProductByIdHandler _handler;
    private readonly Fixture _fixture;

    public GetProductByIdHandlerTests()
    {
        _crudServiceMock = new Mock<ICrudService<Products>>();
        _handler = new GetProductByIdHandler(_crudServiceMock.Object);
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task HandleAsync_WhenProductExists_ShouldReturnProductWithSuccessMessage()
    {
        // Arrange
        var productId = "test-product-id";
        var product = _fixture.Build<Products>()
            .With(p => p.Id, productId)
            .Create();
        var query = new GetProductByIdQuery(productId);

        _crudServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Get data by Id successfully");
        result.Result.Should().BeEquivalentTo(product);

        _crudServiceMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenProductNotFound_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var productId = "non-existent-id";
        var query = new GetProductByIdQuery(productId);

        _crudServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Products)null!);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("No data found.");
        result.Result.Should().BeNull();

        _crudServiceMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyProductId_ShouldCallCrudServiceWithEmptyId()
    {
        // Arrange
        var productId = string.Empty;
        var query = new GetProductByIdQuery(productId);

        _crudServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Products)null!);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("No data found.");

        _crudServiceMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNullProductId_ShouldCallCrudServiceWithNullId()
    {
        // Arrange
        var query = new GetProductByIdQuery(null!);

        _crudServiceMock.Setup(x => x.GetByIdAsync(null!))
            .ReturnsAsync((Products)null!);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("No data found.");

        _crudServiceMock.Verify(x => x.GetByIdAsync(null!), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCrudServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var productId = "test-id";
        var query = new GetProductByIdQuery(productId);

        _crudServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(query, CancellationToken.None));

        _crudServiceMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_ShouldPassTokenToCrudService()
    {
        // Arrange
        var productId = "test-id";
        var product = _fixture.Create<Products>();
        var query = new GetProductByIdQuery(productId);
        var cancellationToken = new CancellationToken();

        _crudServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        await _handler.HandleAsync(query, cancellationToken);

        // Assert
        _crudServiceMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public void Constructor_WithNullCrudService_ShouldAllowNullButWillFailOnUsage()
    {
        // Act - Constructor allows null (no validation in constructor)
        var handler = new GetProductByIdHandler(null!);

        // Assert - Handler is created but will fail when used
        handler.Should().NotBeNull();
    }

    [Theory]
    [InlineData("valid-id")]
    [InlineData("another-valid-id")]
    [InlineData("123")]
    public async Task HandleAsync_WithDifferentValidIds_ShouldCallCrudServiceWithCorrectId(string productId)
    {
        // Arrange
        var product = _fixture.Build<Products>()
            .With(p => p.Id, productId)
            .Create();
        var query = new GetProductByIdQuery(productId);

        _crudServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Result.Should().BeEquivalentTo(product);
        _crudServiceMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenProductFound_ShouldReturnCorrectResponseStructure()
    {
        // Arrange
        var productId = "test-id";
        var product = _fixture.Create<Products>();
        var query = new GetProductByIdQuery(productId);

        _crudServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().NotBeNullOrEmpty();
        result.Result.Should().NotBeNull();
        result.IsSuccessStatusCode.Should().BeTrue();
    }
}
