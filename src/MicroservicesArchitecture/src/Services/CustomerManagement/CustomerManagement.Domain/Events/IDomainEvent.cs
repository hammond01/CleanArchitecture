namespace CustomerManagement.Domain.Events;

public interface IDomainEvent
{
  DateTimeOffset OccurredOn { get; }
}
