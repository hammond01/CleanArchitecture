namespace CleanArchitecture.Api.Auditing;

public interface IAuditOutboxProcessor
{
    Task<int> ProcessPendingMessagesAsync(int batchSize, CancellationToken cancellationToken = default);
}
