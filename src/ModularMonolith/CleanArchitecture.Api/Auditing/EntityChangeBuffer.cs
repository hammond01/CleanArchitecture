using BuildingBlocks.Application.Auditing;

namespace CleanArchitecture.Api.Auditing;

/// <summary>
/// Scoped in-memory buffer for entity changes captured during a request.
/// </summary>
public sealed class EntityChangeBuffer : IEntityChangeBuffer
{
    private readonly List<EntityChange> _changes = new();

    public void CaptureRange(IEnumerable<EntityChange> changes)
    {
        _changes.AddRange(changes);
    }

    public IReadOnlyCollection<EntityChange> Drain()
    {
        var snapshot = _changes.ToArray();
        _changes.Clear();
        return snapshot;
    }
}
