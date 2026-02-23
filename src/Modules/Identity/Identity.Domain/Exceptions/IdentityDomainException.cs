namespace Identity.Domain.Exceptions;

/// <summary>
/// Exception for Identity domain errors
/// </summary>
public class IdentityDomainException : Exception
{
    public IdentityDomainException(string message) : base(message)
    {
    }

    public IdentityDomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
