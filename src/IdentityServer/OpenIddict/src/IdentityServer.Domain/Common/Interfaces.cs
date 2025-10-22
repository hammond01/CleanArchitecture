namespace IdentityServer.Domain.Common;

/// <summary>
/// Base interface for domain entities
/// </summary>
public interface IEntity<TKey>
{
    TKey Id { get; set; }
}

/// <summary>
/// Base interface for auditable entities
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    Guid? UpdatedBy { get; set; }
}

/// <summary>
/// Base interface for soft-deletable entities
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    Guid? DeletedBy { get; set; }
}
