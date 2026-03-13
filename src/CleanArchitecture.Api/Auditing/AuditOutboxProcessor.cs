using Auditing.Domain.Entities;
using Auditing.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Api.Auditing;

public sealed class AuditOutboxProcessor : IAuditOutboxProcessor
{
    private static readonly SemaphoreSlim Gate = new(1, 1);

    private readonly CatalogDbContext _catalogDbContext;
    private readonly IdentityDbContext _identityDbContext;
    private readonly AuditingDbContext _auditingDbContext;
    private readonly ILogger<AuditOutboxProcessor> _logger;

    public AuditOutboxProcessor(
        CatalogDbContext catalogDbContext,
        IdentityDbContext identityDbContext,
        AuditingDbContext auditingDbContext,
        ILogger<AuditOutboxProcessor> logger)
    {
        _catalogDbContext = catalogDbContext;
        _identityDbContext = identityDbContext;
        _auditingDbContext = auditingDbContext;
        _logger = logger;
    }

    public async Task<int> ProcessPendingMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        await Gate.WaitAsync(cancellationToken);

        try
        {
            var processedCount = 0;
            processedCount += await ProcessContextAsync(_catalogDbContext, batchSize, cancellationToken);
            processedCount += await ProcessContextAsync(_identityDbContext, batchSize, cancellationToken);
            return processedCount;
        }
        finally
        {
            Gate.Release();
        }
    }

    private async Task<int> ProcessContextAsync(
        DbContext sourceContext,
        int batchSize,
        CancellationToken cancellationToken)
    {
        var pendingMessages = await sourceContext.Set<AuditOutboxMessage>()
            .Where(x => x.ProcessedDateTime == null)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (pendingMessages.Count == 0)
        {
            return 0;
        }

        try
        {
            var messageIds = pendingMessages.Select(x => x.Id).ToList();
            var existingAuditIds = await _auditingDbContext.AuditLogEntries
                .Where(x => messageIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var existingAuditIdSet = existingAuditIds.ToHashSet(StringComparer.Ordinal);
            foreach (var message in pendingMessages)
            {
                if (!existingAuditIdSet.Contains(message.Id))
                {
                    await _auditingDbContext.AuditLogEntries.AddAsync(MapToAuditEntry(message), cancellationToken);
                }

                message.ProcessedDateTime = DateTimeOffset.UtcNow;
                message.AttemptCount++;
                message.LastError = null;
            }

            await _auditingDbContext.SaveChangesAsync(cancellationToken);
            await sourceContext.SaveChangesAsync(cancellationToken);

            return pendingMessages.Count;
        }
        catch (Exception ex)
        {
            foreach (var message in pendingMessages)
            {
                message.AttemptCount++;
                message.LastError = ex.Message.Length > 2000
                    ? ex.Message[..2000]
                    : ex.Message;
            }

            await sourceContext.SaveChangesAsync(cancellationToken);
            _logger.LogError(ex, "Failed to process audit outbox messages for {ContextType}", sourceContext.GetType().Name);
            return 0;
        }
    }

    private static AuditLogEntry MapToAuditEntry(AuditOutboxMessage message)
    {
        return new AuditLogEntry
        {
            Id = message.Id,
            UserId = message.UserId,
            Action = message.Action,
            ObjectId = message.EntityId,
            Log = message.Log,
            CreatedDateTime = message.OccurredAtUtc,
            UpdatedDateTime = message.ProcessedDateTime
        };
    }
}
