using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Feature.Product.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Constants.ApiResponseConstants;
using System.Data;
using Xunit;

namespace ProductManager.UnitTests.Application.Product;

public class AddOrUpdateProductHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICrudService<Products>> _crudServiceMock;
    private readonly AddOrUpdateProductHandler _handler;
    private readonly Fixture _fixture; public AddOrUpdateProductHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _crudServiceMock = new Mock<ICrudService<Products>>();
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

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<IDisposable>());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.StatusCode.Should().Be(201);
        result.Message.Should().Be(CRUDMessage.CreateSuccess);

        _crudServiceMock.Verify(x => x.AddAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenProductIdExists_ShouldUpdateProduct()
    {
        // Arrange
        var product = _fixture.Build<Products>()
            .With(p => p.Id, "existing-id")
            .Create();
        var command = new AddOrUpdateProductCommand(product);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<IDisposable>());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be(CRUDMessage.UpdateSuccess);

        _crudServiceMock.Verify(x => x.UpdateAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
