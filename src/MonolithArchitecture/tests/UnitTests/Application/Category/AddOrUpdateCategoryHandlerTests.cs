using System.Data;
using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Feature.Category.Commands;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using Xunit;

namespace ProductManager.UnitTests.Application.Category;

public class AddOrUpdateCategoryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICrudService<Categories>> _crudServiceMock;
    private readonly Mock<IDisposable> _transactionMock;
    private readonly AddOrUpdateCategoryHandler _handler;
    private readonly Fixture _fixture;

    public AddOrUpdateCategoryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _crudServiceMock = new Mock<ICrudService<Categories>>();
        _transactionMock = new Mock<IDisposable>();
        _handler = new AddOrUpdateCategoryHandler(_crudServiceMock.Object, _unitOfWorkMock.Object);
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Setup default transaction behavior
        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenCategoryIdIsNull_ShouldCreateNewCategory()
    {
        // Arrange
        var category = _fixture.Build<Categories>()
            .With(c => c.Id, (string)null!)
            .Create();
        var command = new AddOrUpdateCategoryCommand(category);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201);
        result.Message.Should().Be(CRUDMessage.CreateSuccess);
        result.Result.Should().Be(category);

        _crudServiceMock.Verify(x => x.AddAsync(category, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        category.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task HandleAsync_WhenCategoryIdExists_ShouldUpdateCategory()
    {
        // Arrange
        var category = _fixture.Build<Categories>()
            .With(c => c.Id, "existing-id")
            .Create();
        var command = new AddOrUpdateCategoryCommand(category);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be(CRUDMessage.UpdateSuccess);

        _crudServiceMock.Verify(x => x.UpdateAsync(category, It.IsAny<CancellationToken>()), Times.Once);
        _crudServiceMock.Verify(x => x.AddAsync(It.IsAny<Categories>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenAddAsyncThrows_ShouldPropagateException()
    {
        // Arrange
        var category = _fixture.Build<Categories>()
            .With(c => c.Id, (string)null!)
            .Create();
        var command = new AddOrUpdateCategoryCommand(category);
        var exception = new InvalidOperationException("Database error");

        _crudServiceMock.Setup(x => x.AddAsync(It.IsAny<Categories>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act & Assert
        var act = async () => await _handler.HandleAsync(command);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error");

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenUpdateAsyncThrows_ShouldPropagateException()
    {
        // Arrange
        var category = _fixture.Build<Categories>()
            .With(c => c.Id, "existing-id")
            .Create();
        var command = new AddOrUpdateCategoryCommand(category);
        var exception = new InvalidOperationException("Database error");

        _crudServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Categories>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act & Assert
        var act = async () => await _handler.HandleAsync(command);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error");

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenTransactionFails_ShouldNotCommit()
    {
        // Arrange
        var category = _fixture.Build<Categories>()
            .With(c => c.Id, (string)null!)
            .Create();
        var command = new AddOrUpdateCategoryCommand(category);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Transaction failed"));

        // Act & Assert
        var act = async () => await _handler.HandleAsync(command);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Transaction failed");

        _crudServiceMock.Verify(x => x.AddAsync(It.IsAny<Categories>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
