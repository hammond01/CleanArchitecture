namespace ProductManager.Application.Feature.Orders.EventHandlers;

public class OrderUpdatedEventHandler : IDomainEventHandler<EntityUpdatedEvent<Order>>
{
    private readonly ICrudService<AuditLog> _auditService;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<OutboxEvent, string> _outboxEventRepository;

    public OrderUpdatedEventHandler(ICrudService<AuditLog> auditService, ICurrentUser currentUser,
        IRepository<OutboxEvent, string> outboxEventRepository)
    {
        _auditService = auditService;
        _currentUser = currentUser;
        _outboxEventRepository = outboxEventRepository;
    }
    public async Task HandleAsync(EntityUpdatedEvent<Order> domainEvent, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            Id = UlidExtension.Generate(),
            UserId = _currentUser.IsAuthenticated ? _currentUser.UserId : string.Empty,
            CreatedDateTime = domainEvent.EventDateTime,
            Action = EventTypeConstants.OrderUpdated,
            ObjectId = domainEvent.Entity.Id,
            Log = domainEvent.Entity.AsJsonString()
        };

        await _auditService.AddAsync(auditLog, cancellationToken);

        var outboxEvent = new OutboxEvent
        {
            Id = UlidExtension.Generate(),
            EventType = EventTypeConstants.OrderUpdated,
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
