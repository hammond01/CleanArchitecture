namespace ProductManager.Domain.Entities;

public class AuditLog : Entity<string>
{
    public string UserId { get; set; } = default!;

    public string Action { get; set; } = default!;

    public string ObjectId { get; set; } = default!;

    public string Log { get; set; } = default!;
}
