namespace ProductManager.Domain.Entities.Identity;

public class UserSession
{
    public Guid SessionUniqueId { get; set; }

    public string? IP { get; set; }

    public string? Device { get; set; }

    public string? Address { get; set; }

    public DateTimeOffset StartedOn { get; set; }

    public DateTimeOffset? RenewedOn { get; set; }

    public List<string> Roles { get; set; } = [];

    public List<KeyValuePair<string, string>> ExposedClaims { get; set; } = [];
}
