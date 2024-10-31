namespace ProductManager.Domain.Entities;

public class AuditLogEntry : Entity<Guid>
{
    public Guid UserId { get; set; }

    public string Action { get; set; } = default!;

    public string ObjectId { get; set; }= default!;

    public string Log { get; set; }= default!;
}
