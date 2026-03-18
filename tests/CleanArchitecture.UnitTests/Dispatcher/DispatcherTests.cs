using BuildingBlocks.Application;
using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Application.Dispatcher;
using BuildingBlocks.Domain.Events;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchitecture.UnitTests.Dispatcher;

/// <summary>
/// Unit tests for custom Dispatcher
/// </summary>
public class DispatcherTests
{
    [Fact]
    public async Task DispatchAsync_WithValidQuery_ReturnsResult()
    {
        // Arrange
        var query = new TestQuery { Value = "test" };
        var dispatcher = CreateDispatcher(services =>
        {
            services.AddScoped<IQueryHandler<TestQuery, string>, TestQueryHandler>();
        });

        // Act
        var result = await dispatcher.DispatchAsync(query);

        // Assert
        result.Should().Be("test-result");
    }

    [Fact]
    public async Task DispatchAsync_WithValidCommand_ReturnsResult()
    {
        // Arrange
        var command = new TestCommand { Value = "test" };
        var dispatcher = CreateDispatcher(services =>
        {
            services.AddScoped<ICommandHandler<TestCommand, int>, TestCommandHandler>();
        });

        // Act
        var result = await dispatcher.DispatchAsync(command);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public async Task DispatchAsync_WithInvalidQuery_ThrowsValidationException()
    {
        // Arrange
        var query = new TestQuery { Value = string.Empty };
        var dispatcher = CreateDispatcher(services =>
        {
            services.AddScoped<IValidator<TestQuery>, TestQueryValidator>();
            services.AddScoped<IQueryHandler<TestQuery, string>, TestQueryHandler>();
        });

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => dispatcher.DispatchAsync(query));
    }

    [Fact]
    public async Task DispatchAsync_WithInvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new TestCommand { Value = string.Empty };
        var dispatcher = CreateDispatcher(services =>
        {
            services.AddScoped<IValidator<TestCommand>, TestCommandValidator>();
            services.AddScoped<ICommandHandler<TestCommand, int>, TestCommandHandler>();
        });

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => dispatcher.DispatchAsync(command));
    }

    [Fact]
    public async Task DispatchAsync_WithNullQueryResult_ReturnsNull()
    {
        // Arrange
        var query = new NullableResultQuery { Value = "missing" };
        var dispatcher = CreateDispatcher(services =>
        {
            services.AddScoped<IQueryHandler<NullableResultQuery, string?>, NullableResultQueryHandler>();
        });

        // Act
        var result = await dispatcher.DispatchAsync(query);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DispatchAsync_WithMissingQueryHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var query = new TestQuery { Value = "test" };
        var dispatcher = CreateDispatcher();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync(query));
    }

    [Fact]
    public async Task DispatchAsync_WithMissingCommandHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new TestCommand { Value = "test" };
        var dispatcher = CreateDispatcher();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync(command));
    }

    [Fact]
    public async Task DispatchAsync_WithDomainEvent_InvokesMatchingHandlers()
    {
        // Arrange
        var counter = new TestEventCounter();
        var dispatcher = CreateDispatcher(services =>
        {
            services.AddSingleton(counter);
            services.AddScoped<IDomainEventHandler<TestDomainEvent>, TestDomainEventHandlerOne>();
            services.AddScoped<IDomainEventHandler<TestDomainEvent>, TestDomainEventHandlerTwo>();
        });

        // Act
        await dispatcher.DispatchAsync(new TestDomainEvent());

        // Assert
        counter.HandlerOneCount.Should().Be(1);
        counter.HandlerTwoCount.Should().Be(1);
    }

    [Fact]
    public async Task DispatchAsync_WithDomainEvent_IgnoresNonMatchingHandlers()
    {
        // Arrange
        var counter = new TestEventCounter();
        var dispatcher = CreateDispatcher(services =>
        {
            services.AddSingleton(counter);
            services.AddScoped<IDomainEventHandler<TestDomainEvent>, TestDomainEventHandlerOne>();
            services.AddScoped<IDomainEventHandler<OtherDomainEvent>, OtherDomainEventHandler>();
        });

        // Act
        await dispatcher.DispatchAsync(new TestDomainEvent());

        // Assert
        counter.HandlerOneCount.Should().Be(1);
        counter.OtherHandlerCount.Should().Be(0);
    }

    [Fact]
    public async Task AddHandlersFromAssembly_RegistersDomainEventHandlersWithoutDuplicates()
    {
        // Arrange
        var counter = new TestEventCounter();
        var dispatcher = CreateDispatcher(services =>
        {
            services.AddSingleton(counter);
            services.AddHandlersFromAssembly(typeof(DispatcherTests).Assembly);
            services.AddHandlersFromAssembly(typeof(DispatcherTests).Assembly);
        });

        // Act
        await dispatcher.DispatchAsync(new TestDomainEvent());

        // Assert
        counter.HandlerOneCount.Should().Be(1);
        counter.HandlerTwoCount.Should().Be(1);
    }

    private static IDispatcher CreateDispatcher(Action<IServiceCollection>? configureServices = null)
    {
        var services = new ServiceCollection();
        var loggerMock = new Mock<ILogger<BuildingBlocks.Application.Dispatcher.Dispatcher>>();

        services.AddSingleton(loggerMock.Object);

        configureServices?.Invoke(services);

        var serviceProvider = services.BuildServiceProvider();
        return new BuildingBlocks.Application.Dispatcher.Dispatcher(serviceProvider, loggerMock.Object);
    }

    public record TestQuery : IQuery<string>
    {
        public string Value { get; init; } = string.Empty;
    }

    public record TestCommand : ICommand<int>
    {
        public string Value { get; init; } = string.Empty;
    }

    public record NullableResultQuery : IQuery<string?>
    {
        public string Value { get; init; } = string.Empty;
    }

    public sealed class TestQueryHandler : IQueryHandler<TestQuery, string>
    {
        public Task<string> HandleAsync(TestQuery query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult($"{query.Value}-result");
        }
    }

    public sealed class TestCommandHandler : ICommandHandler<TestCommand, int>
    {
        public Task<int> HandleAsync(TestCommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(42);
        }
    }

    public sealed class NullableResultQueryHandler : IQueryHandler<NullableResultQuery, string?>
    {
        public Task<string?> HandleAsync(NullableResultQuery query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string?>(null);
        }
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
