namespace BuildingBlocks.Application.CQRS;

/// <summary>
/// Command handler interface
/// </summary>
/// <typeparam name="TCommand">Command type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface ICommandHandler<in TCommand, TResponse> 
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Command handler without return value
/// </summary>
/// <typeparam name="TCommand">Command type</typeparam>
public interface ICommandHandler<in TCommand> 
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
