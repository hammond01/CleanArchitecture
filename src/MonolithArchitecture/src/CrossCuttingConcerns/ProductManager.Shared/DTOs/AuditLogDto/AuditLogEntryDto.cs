namespace ProductManager.Shared.DTOs.AuditLogDto;

public class AuditLogEntryDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string ObjectId { get; set; } = string.Empty;

    public string Log { get; set; } = string.Empty;

    public DateTimeOffset CreatedDateTime { get; set; }
}
