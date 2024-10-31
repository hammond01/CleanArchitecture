namespace ProductManager.Application.Feature.Category.EventHandlers;

public class CategoryCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<Categories>>
{

    public Task HandleAsync(EntityCreatedEvent<Categories> domainEvent, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
