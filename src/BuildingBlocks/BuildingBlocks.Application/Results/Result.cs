namespace BuildingBlocks.Application.Results;

/// <summary>
/// Result pattern for operation outcomes without return value
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    public static Result Success() => new(true);
    
    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true);
    
    public static Result<T> Failure<T>(string error) => new(default!, false, error);
}

/// <summary>
/// Result pattern for operation outcomes with return value
/// </summary>
/// <typeparam name="T">Value type</typeparam>
public class Result<T> : Result
{
    private readonly T _value;

    protected internal Result(T value, bool isSuccess, string? error = null)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess 
        ? _value 
        : throw new InvalidOperationException("Cannot access value of a failed result");

    public static implicit operator Result<T>(T value) => Success(value);
}
