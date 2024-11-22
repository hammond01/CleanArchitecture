namespace ProductManager.Domain.Entities;

public class UserToken : Entity<Guid>
{
    public Guid UserId { get; set; }

    public string LoginProvider { get; set; } = string.Empty;

    public string TokenName { get; set; } = string.Empty;

    public string TokenValue { get; set; } = string.Empty;
}
