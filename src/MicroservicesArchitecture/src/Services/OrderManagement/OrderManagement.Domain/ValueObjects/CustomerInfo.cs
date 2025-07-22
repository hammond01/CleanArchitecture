namespace OrderManagement.Domain.ValueObjects;

/// <summary>
/// Customer information value object
/// </summary>
public class CustomerInfo : IEquatable<CustomerInfo>
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }

    // Private constructor for EF Core
    private CustomerInfo() { }

    private CustomerInfo(string name, string email, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
    }

    public static CustomerInfo Create(string name, string email, string? phone = null)
    {
        return new CustomerInfo(name, email, phone);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CustomerInfo);
    }

    public bool Equals(CustomerInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Name == other.Name &&
               Email == other.Email &&
               Phone == other.Phone;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Email, Phone);
    }

    public override string ToString()
    {
        return $"{Name} ({Email})";
    }

    public static bool operator ==(CustomerInfo? left, CustomerInfo? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CustomerInfo? left, CustomerInfo? right)
    {
        return !Equals(left, right);
    }
}
