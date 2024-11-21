using ProductManager.Shared.DTOs.AuditLogDto;
namespace ProductManager.Application.Feature.AuditLogEntries.Queries;

public class GetAuditEntriesQuery : AuditLogEntryQueryOptions, IQuery<List<AuditLogEntryDto>>
{
}
internal class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditLogEntryDto>>
{
    private readonly IAuditLogEntryRepository _auditLogEntryRepository;
    private readonly IUserRepository _userRepository;

    public GetAuditEntriesQueryHandler(IAuditLogEntryRepository auditLogEntryRepository, IUserRepository userRepository)
    {
        _auditLogEntryRepository = auditLogEntryRepository;
        _userRepository = userRepository;
    }

    public async Task<List<AuditLogEntryDto>> HandleAsync(GetAuditEntriesQuery query, CancellationToken cancellationToken = default)
    {
        //var auditLogs = _auditLogEntryRepository.Get(query);
        //var users = _userRepository.GetQueryableSet();
        await Task.Delay(1000, cancellationToken);
        // var rs = auditLogs.Join(users, x => x.UserId, y => y.Id,
        // (x, y) => new AuditLogEntryDto
        // {
        //     Id = x.Id,
        //     UserId = x.UserId,
        //     Action = x.Action,
        //     ObjectId = x.ObjectId,
        //     Log = x.Log,
        //     CreatedDateTime = x.CreatedDateTime,
        //     UserName = y.UserName
        // });

        return [];
    }
}
