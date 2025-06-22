using ProductManager.Domain.Entities;

namespace ProductManager.Domain.Repositories;

public class AuditLogEntryQueryOptions
{
    public Guid UserId { get; set; }

    public string ObjectId { get; set; } = string.Empty;

    public bool AsNoTracking { get; set; }
}
public interface IAuditLogEntryRepository : IRepository<AuditLog, string>
{
    IQueryable<AuditLog> Get(AuditLogEntryQueryOptions queryOptions);
}
