namespace BuildingBlocks.Application.Security;

/// <summary>
/// Accesses the current authenticated user in application/infrastructure layers.
/// </summary>
public interface ICurrentUserAccessor
{
    string? UserId { get; }
    string? UserName { get; }
    string? RequestMethod { get; }
    string? RequestPath { get; }
    string? QueryString { get; }
    string? TraceIdentifier { get; }
}
