namespace BuildingBlocks.Application.Auditing;

/// <summary>
/// Captures a single persisted entity change within the current request scope.
/// </summary>
public sealed record EntityChange
{
    public string ModuleName { get; init; } = null!;
    public string EntityName { get; init; } = null!;
    public string EntityId { get; init; } = null!;
    public string ChangeType { get; init; } = null!;
    public string UserId { get; init; } = null!;
    public string? RequestMethod { get; init; }
    public string? RequestPath { get; init; }
    public string? QueryString { get; init; }
    public string? TraceIdentifier { get; init; }
    public string ChangesJson { get; init; } = null!;
    public DateTimeOffset OccurredAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
