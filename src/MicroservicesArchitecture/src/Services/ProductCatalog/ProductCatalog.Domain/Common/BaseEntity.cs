using Shared.Common.Mediator;

namespace ProductCatalog.Domain.Common;

/// <summary>
/// Base entity with common properties
/// </summary>
public abstract class BaseEntity
{
    public string Id { get; protected set; } = string.Empty;
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public string? UpdatedBy { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
    {
        Id = GenerateId();
        CreatedAt = DateTime.UtcNow;
    }

    protected BaseEntity(string id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void SetCreatedBy(string createdBy)
    {
        CreatedBy = createdBy;
    }

    public void SetUpdatedBy(string updatedBy)
    {
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateId()
    {
        // Generate ULID for better performance and sorting
        return Ulid.NewUlid().ToString();
    }
}

/// <summary>
/// Domain event interface
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
    string EventId { get; }
}

/// <summary>
/// Base domain event
/// </summary>
public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }
    public string EventId { get; }

    protected BaseDomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
        EventId = Ulid.NewUlid().ToString();
    }
}
