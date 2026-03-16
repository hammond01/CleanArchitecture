using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Domain.Entities;

/// <summary>
/// Base entity class for all domain entities
/// </summary>
/// <typeparam name="TKey">Type of the primary key</typeparam>
public abstract class Entity<TKey> : IHasKey<TKey>, ITrackable, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public TKey Id { get; set; } = default!;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RowId { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
