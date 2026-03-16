namespace BuildingBlocks.Application.CQRS;

/// <summary>
/// Query interface for read operations
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IQuery<out TResponse>
{
}
