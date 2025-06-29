namespace ProductManager.Shared.Exceptions;

public class ValidationException : Exception
{

    public ValidationException(string message)
        : base(message)
    {
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    public static void Requires(bool expected, string errorMessage)
    {
        if (!expected)
        {
            throw new ValidationException(errorMessage);
        }
    }
}
