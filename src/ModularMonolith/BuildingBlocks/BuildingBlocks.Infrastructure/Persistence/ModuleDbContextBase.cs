using System.Data;
using System.Text.Json;
using BuildingBlocks.Application.Auditing;
using BuildingBlocks.Application.Dispatcher;
using BuildingBlocks.Application.Security;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Base DbContext for module persistence with audit capture and domain event dispatch.
/// </summary>
public abstract class ModuleDbContextBase : DbContext, IUnitOfWork
{
    private static readonly HashSet<string> IgnoredAuditProperties = new(StringComparer.Ordinal)
    {
        "RowId",
        "CreatedDateTime",
        "UpdatedDateTime"
    };

    private readonly IDispatcher? _dispatcher;
    private readonly IEntityChangeBuffer? _entityChangeBuffer;
    private readonly ICurrentUserAccessor? _currentUserAccessor;

    protected ModuleDbContextBase(
        DbContextOptions options,
        IDispatcher? dispatcher = null,
        IEntityChangeBuffer? entityChangeBuffer = null,
        ICurrentUserAccessor? currentUserAccessor = null)
        : base(options)
    {
        _dispatcher = dispatcher;
        _entityChangeBuffer = entityChangeBuffer;
        _currentUserAccessor = currentUserAccessor;
    }

    protected abstract string ModuleName { get; }

    protected virtual string ModuleSchema => ModuleName.ToLowerInvariant();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureAuditOutbox(modelBuilder.Entity<AuditOutboxMessage>());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entityChanges = CaptureEntityChanges();
        var auditOutboxMessages = entityChanges
            .Select(CreateAuditOutboxMessage)
            .ToList();

        if (auditOutboxMessages.Count > 0)
        {
            Set<AuditOutboxMessage>().AddRange(auditOutboxMessages);
        }

        var domainEventOwners = ChangeTracker.Entries()
            .Select(x => x.Entity)
            .OfType<IHasDomainEvents>()
            .Where(x => x.DomainEvents.Count > 0)
            .Distinct()
            .ToList();

        var domainEvents = domainEventOwners
            .SelectMany(x => x.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        if (entityChanges.Count > 0)
        {
            _entityChangeBuffer?.CaptureRange(entityChanges);
        }

        if (_dispatcher != null)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _dispatcher.DispatchAsync(domainEvent, cancellationToken);
            }
        }

        foreach (var owner in domainEventOwners)
        {
            owner.ClearDomainEvents();
        }

        return result;
    }

    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await SaveChangesAsync(cancellationToken);
    }

    async Task<IDisposable> IUnitOfWork.BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    async Task<IDisposable> IUnitOfWork.BeginTransactionAsync(
        IsolationLevel isolationLevel,
        string? lockName,
        CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    async Task IUnitOfWork.CommitTransactionAsync(CancellationToken cancellationToken)
    {
        var transaction = Database.CurrentTransaction;
        if (transaction == null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private List<EntityChange> CaptureEntityChanges()
    {
        var userId = _currentUserAccessor?.UserId ?? "system";

        return ChangeTracker.Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(entry => entry.Entity is not null)
            .Where(entry => entry.Entity is not AuditOutboxMessage)
            .Select(entry => CreateEntityChange(entry, userId))
            .Where(change => change != null)
            .Cast<EntityChange>()
            .ToList();
    }

    private EntityChange? CreateEntityChange(EntityEntry entry, string userId)
    {
        var properties = entry.Properties
            .Where(property => !IgnoredAuditProperties.Contains(property.Metadata.Name))
            .ToList();

        object payload = entry.State switch
        {
            EntityState.Added => new
            {
                after = properties.ToDictionary(
                    property => property.Metadata.Name,
                    property => property.CurrentValue)
            },
            EntityState.Modified => new
            {
                before = properties
                    .Where(property => property.IsModified)
                    .ToDictionary(property => property.Metadata.Name, property => property.OriginalValue),
                after = properties
                    .Where(property => property.IsModified)
                    .ToDictionary(property => property.Metadata.Name, property => property.CurrentValue)
            },
            EntityState.Deleted => new
            {
                before = properties.ToDictionary(
                    property => property.Metadata.Name,
                    property => property.OriginalValue)
            },
            _ => new { }
        };

        var primaryKey = entry.Metadata.FindPrimaryKey();
        var entityId = primaryKey == null
            ? entry.Entity.GetHashCode().ToString()
            : string.Join(":", primaryKey.Properties.Select(property =>
                entry.Property(property.Name).CurrentValue
                ?? entry.Property(property.Name).OriginalValue
                ?? string.Empty));

        return new EntityChange
        {
            ModuleName = ModuleName,
            EntityName = entry.Metadata.ClrType.Name,
            EntityId = entityId,
            ChangeType = entry.State.ToString(),
            UserId = userId,
            RequestMethod = _currentUserAccessor?.RequestMethod,
            RequestPath = _currentUserAccessor?.RequestPath,
            QueryString = _currentUserAccessor?.QueryString,
            TraceIdentifier = _currentUserAccessor?.TraceIdentifier,
            ChangesJson = JsonSerializer.Serialize(payload),
            OccurredAtUtc = DateTimeOffset.UtcNow
        };
    }

    private AuditOutboxMessage CreateAuditOutboxMessage(EntityChange entityChange)
    {
        var action = !string.IsNullOrWhiteSpace(entityChange.RequestMethod)
            && !string.IsNullOrWhiteSpace(entityChange.RequestPath)
            ? $"{entityChange.RequestMethod} {entityChange.RequestPath}"
            : $"{entityChange.ChangeType} {entityChange.EntityName}";

        var log = JsonSerializer.Serialize(new
        {
            module = entityChange.ModuleName,
            entity = entityChange.EntityName,
            changeType = entityChange.ChangeType,
            path = entityChange.RequestPath,
            queryString = entityChange.QueryString,
            traceIdentifier = entityChange.TraceIdentifier,
            changes = JsonSerializer.Deserialize<JsonElement>(entityChange.ChangesJson)
        });

        return new AuditOutboxMessage
        {
            ModuleName = entityChange.ModuleName,
            EntityName = entityChange.EntityName,
            EntityId = entityChange.EntityId,
            ChangeType = entityChange.ChangeType,
            UserId = entityChange.UserId,
            Action = action,
            Log = log,
            OccurredAtUtc = entityChange.OccurredAtUtc
        };
    }

    private void ConfigureAuditOutbox(EntityTypeBuilder<AuditOutboxMessage> builder)
    {
        builder.ToTable("audit_outbox_messages", ModuleSchema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.ModuleName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.EntityId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.ChangeType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Log)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.OccurredAtUtc)
            .IsRequired();

        builder.Property(x => x.LastError)
            .HasMaxLength(2000);

        builder.HasIndex(x => new { x.ProcessedDateTime, x.OccurredAtUtc });
    }
}
