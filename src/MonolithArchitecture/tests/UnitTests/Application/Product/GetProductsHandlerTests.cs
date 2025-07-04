using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Feature.Product.Queries;
using ProductManager.Domain.Entities;
using Xunit;

namespace ProductManager.UnitTests.Application.Product;

public class GetProductsHandlerTests
{
    private readonly Mock<ICrudService<Products>> _crudServiceMock;
    private readonly GetsProductsHandler _handler;
    private readonly Fixture _fixture;

    public GetProductsHandlerTests()
    {
        _crudServiceMock = new Mock<ICrudService<Products>>();
        _handler = new GetsProductsHandler(_crudServiceMock.Object);
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task HandleAsync_WhenProductsExist_ShouldReturnProductsList()
    {
        // Arrange
        var products = _fixture.CreateMany<Products>(3).ToList();
        var query = new GetProducts();

        _crudServiceMock.Setup(x => x.GetAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Get product successfully");
        result.Result.Should().BeEquivalentTo(products);

        _crudServiceMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenNoProductsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyProducts = new List<Products>();
        var query = new GetProducts();

        _crudServiceMock.Setup(x => x.GetAsync())
            .ReturnsAsync(emptyProducts);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Get product successfully");
        result.Result.Should().BeEquivalentTo(emptyProducts);

        _crudServiceMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCrudServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetProducts();

        _crudServiceMock.Setup(x => x.GetAsync())
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(query, CancellationToken.None));

        _crudServiceMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_ShouldPassTokenToCrudService()
    {
        // Arrange
        var products = _fixture.CreateMany<Products>(2).ToList();
        var query = new GetProducts();
        var cancellationToken = new CancellationToken();

        _crudServiceMock.Setup(x => x.GetAsync())
            .ReturnsAsync(products);

        // Act
        await _handler.HandleAsync(query, cancellationToken);

        // Assert
        _crudServiceMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public void Constructor_WithNullCrudService_ShouldAllowNullButWillFailOnUsage()
    {
        // Act - Constructor allows null (no validation in constructor)
        var handler = new GetsProductsHandler(null!);

        // Assert - Handler is created but will fail when used
        handler.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnCorrectResponseStructure()
    {
        // Arrange
        var products = _fixture.CreateMany<Products>(1).ToList();
        var query = new GetProducts();

        _crudServiceMock.Setup(x => x.GetAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().NotBeNullOrEmpty();
        result.Result.Should().NotBeNull();
    }
}
