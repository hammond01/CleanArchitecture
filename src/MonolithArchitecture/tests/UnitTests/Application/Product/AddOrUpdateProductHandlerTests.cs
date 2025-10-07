using System.Data;
using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Feature.Product.Commands;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using Xunit;

namespace ProductManager.UnitTests.Application.Product;

public class AddOrUpdateProductHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICrudService<Products>> _crudServiceMock;
    private readonly Mock<IDisposable> _transactionMock;
    private readonly AddOrUpdateProductHandler _handler;
    private readonly Fixture _fixture;

    public AddOrUpdateProductHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _crudServiceMock = new Mock<ICrudService<Products>>();
        _transactionMock = new Mock<IDisposable>();
        _handler = new AddOrUpdateProductHandler(_unitOfWorkMock.Object, _crudServiceMock.Object);
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task HandleAsync_WhenProductIdIsNull_ShouldCreateNewProduct()
    {
        // Arrange
        var product = _fixture.Build<Products>()
            .With(p => p.Id, (string)null!)
            .Create();
        var command = new AddOrUpdateProductCommand(product);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201);
        result.Message.Should().Be(CRUDMessage.CreateSuccess);
        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<Products>();

        var resultProduct = result.Result as Products;
        resultProduct!.Id.Should().NotBeNullOrEmpty();
        resultProduct.ProductName.Should().Be(product.ProductName);

        _crudServiceMock.Verify(x => x.AddAsync(It.Is<Products>(p => p.Id != null), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _transactionMock.Verify(x => x.Dispose(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenProductIdExists_ShouldUpdateProduct()
    {
        // Arrange
        var existingId = "existing-product-id";
        var product = _fixture.Build<Products>()
            .With(p => p.Id, existingId)
            .Create();
        var command = new AddOrUpdateProductCommand(product);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be(CRUDMessage.UpdateSuccess);
        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<Products>();

        var resultProduct = result.Result as Products;
        resultProduct!.Id.Should().Be(existingId);
        resultProduct.ProductName.Should().Be(product.ProductName);

        _crudServiceMock.Verify(x => x.UpdateAsync(It.Is<Products>(p => p.Id == existingId), It.IsAny<CancellationToken>()), Times.Once);
        _crudServiceMock.Verify(x => x.AddAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _transactionMock.Verify(x => x.Dispose(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenCrudServiceThrowsException_ShouldThrowException()
    {
        // Arrange
        var product = _fixture.Build<Products>()
            .With(p => p.Id, (string)null!)
            .Create();
        var command = new AddOrUpdateProductCommand(product);

        _crudServiceMock.Setup(x => x.AddAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));

        exception.Message.Should().Be("Database error");
    }

}
