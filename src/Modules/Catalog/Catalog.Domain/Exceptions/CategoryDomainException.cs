namespace Catalog.Domain.Exceptions;

/// <summary>
/// Exception for Category domain violations
/// </summary>
public class CategoryDomainException : Exception
{
    public CategoryDomainException() { }

    public CategoryDomainException(string message) : base(message) { }

    public CategoryDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
