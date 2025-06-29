using ProductManager.Application.Common.Services;
using ProductManager.Constants;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Events;
using ProductManager.Domain.Identity;
using ProductManager.Domain.Repositories;
using ProductManager.Persistence.Extensions;
using ProductManager.Shared.ExtensionMethods;

namespace ProductManager.Application.Feature.Shipper.EventHandlers;

public class ShipperCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<Shippers>>
{
    private readonly IRepository<AuditLog, string> _auditLogRepository;
    private readonly ICurrentUser _currentUser;
    private readonly UserService _userService;

    public ShipperCreatedEventHandler(
        IRepository<AuditLog, string> auditLogRepository,
        ICurrentUser currentUser,
        UserService userService)
    {
        _auditLogRepository = auditLogRepository;
        _currentUser = currentUser;
        _userService = userService;
    }

    public async Task HandleAsync(EntityCreatedEvent<Shippers> domainEvent, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            Id = UlidExtension.Generate(),
            UserId = _currentUser.UserId,
            Action = EventTypeConstants.ShipperCreated,
            ObjectId = domainEvent.Entity.Id,
            Log = domainEvent.Entity.AsJsonString()
        };

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
        await _auditLogRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
