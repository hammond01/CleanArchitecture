namespace Shared.Common.Exceptions;

/// <summary>
/// Base domain exception
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Business rule validation exception
/// </summary>
public class BusinessRuleValidationException : DomainException
{
    public string RuleName { get; }

    public BusinessRuleValidationException(string ruleName, string message) : base(message)
    {
        RuleName = ruleName;
    }
}

/// <summary>
/// Entity not found exception
/// </summary>
public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityName, object entityId) 
        : base($"{entityName} with id '{entityId}' was not found.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}

/// <summary>
/// Duplicate entity exception
/// </summary>
public class DuplicateEntityException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }

    public DuplicateEntityException(string entityName, object entityId) 
        : base($"{entityName} with id '{entityId}' already exists.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}

/// <summary>
/// Invalid operation exception
/// </summary>
public class InvalidOperationDomainException : DomainException
{
    public InvalidOperationDomainException(string message) : base(message)
    {
    }

    public InvalidOperationDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
