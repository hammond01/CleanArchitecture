using Auditing.Application.DTOs;
using Auditing.Domain.Repositories;
using BuildingBlocks.Application.CQRS;

namespace Auditing.Application.Features.AuditLogEntries.Queries;

/// <summary>
/// Handler for GetAuditEntriesQuery
/// </summary>
public class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditLogEntryDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditEntriesQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<List<AuditLogEntryDto>> HandleAsync(GetAuditEntriesQuery query, CancellationToken cancellationToken = default)
    {
        var entries = await _auditLogRepository.GetEntriesAsync(query.PageNumber, query.PageSize, cancellationToken);

        return entries
            .Select(e => new AuditLogEntryDto
            {
                Id = e.Id,
                UserId = e.UserId,
                Action = e.Action,
                ObjectId = e.ObjectId,
                Log = e.Log,
                CreatedDateTime = e.CreatedDateTime
            })
            .ToList();
    }
}
