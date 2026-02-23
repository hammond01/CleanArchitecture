using Auditing.Application.DTOs;
using BuildingBlocks.Application.CQRS;

namespace Auditing.Application.Features.AuditLogEntries.Queries;

/// <summary>
/// Query to get paginated audit log entries
/// </summary>
public record GetAuditEntriesQuery : IQuery<List<AuditLogEntryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
