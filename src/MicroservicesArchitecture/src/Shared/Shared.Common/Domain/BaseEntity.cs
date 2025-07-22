namespace Shared.Common.Domain;

/// <summary>
/// Base entity class
/// </summary>
public abstract class BaseEntity
{
    public string Number { get; protected set; } = string.Empty;
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    protected BaseEntity()
    {
        Number = GenerateId();
    }

    protected BaseEntity(string id)
    {
        Number = id;
    }

    private static string GenerateId()
    {
        // Generate ULID-like ID using timestamp + random
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var random = Guid.NewGuid().ToString("N")[..10].ToUpper();
        return $"{timestamp:X}{random}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Number == other.Number;
    }

    public override int GetHashCode()
    {
        return Number.GetHashCode();
    }

    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
        return !Equals(left, right);
    }
}
