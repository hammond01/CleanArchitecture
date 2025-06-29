using ProductManager.Application.Common.Services;
using ProductManager.Constants;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Events;
using ProductManager.Domain.Identity;
using ProductManager.Domain.Repositories;
using ProductManager.Persistence.Extensions;
using ProductManager.Shared.ExtensionMethods;
namespace ProductManager.Application.Feature.Product.EventHandlers;

public class ProductUpdatedEventHandler : IDomainEventHandler<EntityUpdatedEvent<Products>>
{
    private readonly ICrudService<AuditLog> _auditService;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<OutboxEvent, string> _outboxEventRepository;

    public ProductUpdatedEventHandler(ICrudService<AuditLog> auditService, ICurrentUser currentUser,
        IRepository<OutboxEvent, string> outboxEventRepository)
    {
        _auditService = auditService;
        _currentUser = currentUser;
        _outboxEventRepository = outboxEventRepository;
    }
    public async Task HandleAsync(EntityUpdatedEvent<Products> domainEvent, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            Id = UlidExtension.Generate(),
            UserId = _currentUser.IsAuthenticated ? _currentUser.UserId : string.Empty,
            CreatedDateTime = domainEvent.EventDateTime,
            Action = EventTypeConstants.ProductUpdated,
            ObjectId = domainEvent.Entity.Id,
            Log = domainEvent.Entity.AsJsonString()
        };

        await _auditService.AddAsync(auditLog, cancellationToken);

        var outboxEvent = new OutboxEvent
        {
            Id = UlidExtension.Generate(),
            EventType = EventTypeConstants.ProductUpdated,
            TriggeredById = _currentUser.UserId,
            CreatedDateTime = domainEvent.EventDateTime,
            ObjectId = domainEvent.Entity.Id,
            Message = domainEvent.Entity.AsJsonString(),
            Published = false
        };

        await _outboxEventRepository.AddAsync(outboxEvent, cancellationToken);
        await _outboxEventRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
