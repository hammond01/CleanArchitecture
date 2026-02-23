using BuildingBlocks.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user logs in
/// </summary>
public class UserLoginEvent : IDomainEvent
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public DateTimeOffset LoginDateTime { get; set; } = DateTimeOffset.UtcNow;
}
