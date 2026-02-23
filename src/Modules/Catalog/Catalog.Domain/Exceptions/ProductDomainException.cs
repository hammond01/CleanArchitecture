namespace Catalog.Domain.Exceptions;

/// <summary>
/// Exception for Product domain violations
/// </summary>
public class ProductDomainException : Exception
{
    public ProductDomainException() { }

    public ProductDomainException(string message) : base(message) { }

    public ProductDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
