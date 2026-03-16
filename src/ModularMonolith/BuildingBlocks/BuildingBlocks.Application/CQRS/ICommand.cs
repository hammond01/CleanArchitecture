namespace BuildingBlocks.Application.CQRS;

/// <summary>
/// Marker interface for commands (write operations)
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Command with return value
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface ICommand<out TResponse> : ICommand
{
}
