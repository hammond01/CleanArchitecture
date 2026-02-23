using BuildingBlocks.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user registers
/// </summary>
public class UserRegisteredEvent : IDomainEvent
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTimeOffset RegisteredDateTime { get; set; } = DateTimeOffset.UtcNow;
}
