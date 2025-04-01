namespace ProductManager.Shared.Logging;

public interface ILogger
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? exception = null);
    void LogError(string exception, string? message, params object?[] args);
    void LogDebug(string message);
    void LogTrace(string message);
    void LogCritical(string message, Exception? exception = null);
}
