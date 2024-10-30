namespace ProductManager.Domain.Entities;

public abstract class Entity<TKey> : IHasKey<TKey>, ITrackable
{
    public TKey Id { get; set; } = default!;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RowId { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }
}
