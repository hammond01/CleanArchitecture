using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Application.Dispatcher;
using BuildingBlocks.Domain.Events;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace CleanArchitecture.UnitTests.Dispatcher;

/// <summary>
/// Unit tests for Custom Dispatcher
/// Tests the core CQRS dispatch functionality with HandleAsync method
/// </summary>
public class DispatcherTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<BuildingBlocks.Application.Dispatcher.Dispatcher>> _loggerMock;

    public DispatcherTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<BuildingBlocks.Application.Dispatcher.Dispatcher>>();
    }

    [Fact]
    public async Task DispatchAsync_WithValidQuery_ReturnsResult()
    {
        // Arrange
        var query = new TestQuery { Value = "test" };
        var expectedResult = "test-result";
        var handlerMock = new Mock<IQueryHandler<TestQuery, string>>();

        handlerMock
            .Setup(h => h.HandleAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IQueryHandler<TestQuery, string>)))
            .Returns(handlerMock.Object);

        // No validators for query (validation passes)
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IValidator<TestQuery>>)))
            .Returns(Enumerable.Empty<IValidator<TestQuery>>());

        var dispatcher = new BuildingBlocks.Application.Dispatcher.Dispatcher(
            _serviceProviderMock.Object,
            _loggerMock.Object);

        // Act
        var result = await dispatcher.DispatchAsync(query);

        // Assert
        result.Should().Be(expectedResult);
        handlerMock.Verify(h => h.HandleAsync(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WithValidCommand_ReturnsResult()
    {
        // Arrange
        var command = new TestCommand { Value = "test" };
        var expectedResult = 42;
        var handlerMock = new Mock<ICommandHandler<TestCommand, int>>();

        handlerMock
            .Setup(h => h.HandleAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<TestCommand, int>)))
            .Returns(handlerMock.Object);

        // No validators (validation passes)
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IValidator<TestCommand>>)))
            .Returns(Enumerable.Empty<IValidator<TestCommand>>());

        var dispatcher = new BuildingBlocks.Application.Dispatcher.Dispatcher(
            _serviceProviderMock.Object,
            _loggerMock.Object);

        // Act
        var result = await dispatcher.DispatchAsync(command);

        // Assert
        result.Should().Be(expectedResult);
        handlerMock.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WithInvalidQuery_ThrowsValidationException()
    {
        // Arrange
        var query = new TestQuery { Value = string.Empty };
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IValidator<TestQuery>>)))
            .Returns(new IValidator<TestQuery>[] { new TestQueryValidator() });

        var dispatcher = new BuildingBlocks.Application.Dispatcher.Dispatcher(
            _serviceProviderMock.Object,
            _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => dispatcher.DispatchAsync(query));
    }

    [Fact]
    public async Task DispatchAsync_WithInvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new TestCommand { Value = "" };
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IValidator<TestCommand>>)))
            .Returns(new IValidator<TestCommand>[] { new TestCommandValidator() });

        var dispatcher = new BuildingBlocks.Application.Dispatcher.Dispatcher(
            _serviceProviderMock.Object,
            _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => dispatcher.DispatchAsync(command));
    }

    [Fact]
    public async Task DispatchAsync_WithNullQueryHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var query = new TestQuery { Value = "test" };

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IQueryHandler<TestQuery, string>)))
            .Returns((IQueryHandler<TestQuery, string>?)null);

        // No validators
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IValidator<TestQuery>>)))
            .Returns(Enumerable.Empty<IValidator<TestQuery>>());

        var dispatcher = new BuildingBlocks.Application.Dispatcher.Dispatcher(
            _serviceProviderMock.Object,
            _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync(query));
    }

    [Fact]
    public async Task DispatchAsync_WithNullCommandHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new TestCommand { Value = "test" };

        // No validators
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IValidator<TestCommand>>)))
            .Returns(Enumerable.Empty<IValidator<TestCommand>>());

        // No handler
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<TestCommand, int>)))
            .Returns((ICommandHandler<TestCommand, int>?)null);

        var dispatcher = new BuildingBlocks.Application.Dispatcher.Dispatcher(
            _serviceProviderMock.Object,
            _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync(command));
    }

    [Fact]
    public async Task DispatchAsync_WithDomainEvent_InvokesMatchingHandlers()
    {
        // Arrange
        var domainEvent = new TestDomainEvent();
        var counter = new TestEventCounter();
        var handlerOne = new TestDomainEventHandlerOne(counter);
        var handlerTwo = new TestDomainEventHandlerTwo(counter);

        ResetEventHandlers(
            typeof(TestDomainEventHandlerOne),
            typeof(TestDomainEventHandlerTwo));

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(TestDomainEventHandlerOne)))
            .Returns(handlerOne);

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(TestDomainEventHandlerTwo)))
            .Returns(handlerTwo);

        var dispatcher = new BuildingBlocks.Application.Dispatcher.Dispatcher(
            _serviceProviderMock.Object,
            _loggerMock.Object);

        // Act
        await dispatcher.DispatchAsync(domainEvent);

        // Assert
        counter.HandlerOneCount.Should().Be(1);
        counter.HandlerTwoCount.Should().Be(1);
    }

    [Fact]
    public async Task DispatchAsync_WithDomainEvent_IgnoresNonMatchingHandlers()
    {
        // Arrange
        var domainEvent = new TestDomainEvent();
        var counter = new TestEventCounter();
        var matchingHandler = new TestDomainEventHandlerOne(counter);
        var nonMatchingHandler = new OtherDomainEventHandler(counter);

        ResetEventHandlers(
            typeof(TestDomainEventHandlerOne),
            typeof(OtherDomainEventHandler));

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(TestDomainEventHandlerOne)))
            .Returns(matchingHandler);

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(OtherDomainEventHandler)))
            .Returns(nonMatchingHandler);

        var dispatcher = new BuildingBlocks.Application.Dispatcher.Dispatcher(
            _serviceProviderMock.Object,
            _loggerMock.Object);

        // Act
        await dispatcher.DispatchAsync(domainEvent);

        // Assert
        counter.HandlerOneCount.Should().Be(1);
        counter.OtherHandlerCount.Should().Be(0);
    }

    // Test helper classes
    public record TestQuery : IQuery<string>
    {
        public string Value { get; init; } = string.Empty;
    }

    public record TestCommand : ICommand<int>
    {
        public string Value { get; init; } = string.Empty;
    }

    public sealed class TestQueryValidator : AbstractValidator<TestQuery>
    {
        public TestQueryValidator()
        {
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required");
        }
    }

    public sealed class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required");
        }
    }

    private static void ResetEventHandlers(params Type[] handlerTypes)
    {
        var field = typeof(BuildingBlocks.Application.Dispatcher.Dispatcher)
            .GetField("_eventHandlers", BindingFlags.NonPublic | BindingFlags.Static);
        var handlers = (List<Type>)field!.GetValue(null)!;
        handlers.Clear();
        handlers.AddRange(handlerTypes);
    }

    public sealed class TestDomainEvent : IDomainEvent
    {
    }

    public sealed class OtherDomainEvent : IDomainEvent
    {
    }

    public sealed class TestEventCounter
    {
        public int HandlerOneCount { get; set; }
        public int HandlerTwoCount { get; set; }
        public int OtherHandlerCount { get; set; }
    }

    public sealed class TestDomainEventHandlerOne : IDomainEventHandler<TestDomainEvent>
    {
        private readonly TestEventCounter _counter;

        public TestDomainEventHandlerOne(TestEventCounter counter)
        {
            _counter = counter;
        }

        public Task HandleAsync(TestDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _counter.HandlerOneCount++;
            return Task.CompletedTask;
        }
    }

    public sealed class TestDomainEventHandlerTwo : IDomainEventHandler<TestDomainEvent>
    {
        private readonly TestEventCounter _counter;

        public TestDomainEventHandlerTwo(TestEventCounter counter)
        {
            _counter = counter;
        }

        public Task HandleAsync(TestDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _counter.HandlerTwoCount++;
            return Task.CompletedTask;
        }
    }

    public sealed class OtherDomainEventHandler : IDomainEventHandler<OtherDomainEvent>
    {
        private readonly TestEventCounter _counter;

        public OtherDomainEventHandler(TestEventCounter counter)
        {
            _counter = counter;
        }

        public Task HandleAsync(OtherDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _counter.OtherHandlerCount++;
            return Task.CompletedTask;
        }
    }
}
