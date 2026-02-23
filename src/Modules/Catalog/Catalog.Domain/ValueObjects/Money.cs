using Catalog.Domain.Exceptions;

namespace Catalog.Domain.ValueObjects;

/// <summary>
/// Money value object representing monetary amount with currency
/// </summary>
public class Money : IEquatable<Money>
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    // Private constructor for EF Core
    private Money()
    {
        Currency = "USD";
    }

    // Factory method
    public static Money Create(decimal amount, string currency = "USD")
    {
        if (amount < 0)
        {
            throw new ProductDomainException("Amount cannot be negative");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ProductDomainException("Currency is required");
        }

        if (currency.Length != 3)
        {
            throw new ProductDomainException("Currency must be a 3-letter ISO code");
        }

        return new Money
        {
            Amount = Math.Round(amount, 2),
            Currency = currency.ToUpperInvariant()
        };
    }

    // Business methods
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new ProductDomainException("Cannot add money with different currencies");
        }

        return Create(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new ProductDomainException("Cannot subtract money with different currencies");
        }

        return Create(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        return Create(Amount * factor, Currency);
    }

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal factor) => money.Multiply(factor);

    // Equality implementation
    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        return obj is Money money && Equals(money);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public static bool operator ==(Money? left, Money? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }
}
