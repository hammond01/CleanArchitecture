namespace ProductCatalog.Domain.Common;

/// <summary>
/// Base entity class
/// </summary>
public abstract class BaseEntity
{
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public string? CreatedBy { get; set; }
  public string? UpdatedBy { get; set; }
}

/// <summary>
/// Base aggregate root
/// </summary>
public abstract class AggregateRoot : BaseEntity
{
  private readonly List<IDomainEvent> _domainEvents = new();

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

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
}

/// <summary>
/// Domain event interface
/// </summary>
public interface IDomainEvent
{
  Guid Id { get; }
  DateTime OccurredOn { get; }
}
