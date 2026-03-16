namespace BuildingBlocks.Application.Auditing;

/// <summary>
/// Scoped buffer used to collect entity changes during a request.
/// </summary>
public interface IEntityChangeBuffer
{
    void CaptureRange(IEnumerable<EntityChange> changes);

    IReadOnlyCollection<EntityChange> Drain();
}
