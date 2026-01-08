namespace BuildingBlocks.Application.Validation;

/// <summary>
/// Validator interface for input validation
/// </summary>
/// <typeparam name="T">Type to validate</typeparam>
public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}

/// <summary>
/// Validation result
/// </summary>
public class ValidationResult
{
    private readonly List<string> _errors = new();

    public bool IsValid => !_errors.Any();
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    public void AddError(string error)
    {
        _errors.Add(error);
    }

    public void AddErrors(IEnumerable<string> errors)
    {
        _errors.AddRange(errors);
    }

    public static ValidationResult Success() => new();
    
    public static ValidationResult Failure(params string[] errors)
    {
        var result = new ValidationResult();
        result.AddErrors(errors);
        return result;
    }
}
