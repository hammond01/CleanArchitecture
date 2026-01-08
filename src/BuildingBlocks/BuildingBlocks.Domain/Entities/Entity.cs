using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingBlocks.Domain.Entities;

/// <summary>
/// Base entity class for all domain entities
/// </summary>
/// <typeparam name="TKey">Type of the primary key</typeparam>
public abstract class Entity<TKey> : IHasKey<TKey>, ITrackable
{
    public TKey Id { get; set; } = default!;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RowId { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }
}
