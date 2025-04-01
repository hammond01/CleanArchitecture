namespace ProductManager.Shared.Logging;

public abstract class LoggerBase : ILogger
{
    protected readonly LogLevel MinimumLevel;

    protected LoggerBase(LogLevel minimumLevel = LogLevel.Information)
    {
        MinimumLevel = minimumLevel;
    }

    public virtual void LogInformation(string message)
    {
        if (MinimumLevel <= LogLevel.Information)
        {
            Log(new LogEntry(LogLevel.Information, message));
        }
    }

    public virtual void LogWarning(string message)
    {
        if (MinimumLevel <= LogLevel.Warning)
        {
            Log(new LogEntry(LogLevel.Warning, message));
        }
    }

    public virtual void LogError(string message, Exception? exception = null)
    {
        if (MinimumLevel <= LogLevel.Error)
        {
            Log(new LogEntry(LogLevel.Error, message, exception));
        }
    }
    public void LogError(string exception, string? message, params object?[] args)
    {
        if (MinimumLevel > LogLevel.Error)
        {
            return;
        }
        var formattedMessage = string.Format(message ?? string.Empty, args);
        Log(new LogEntry(LogLevel.Error, formattedMessage));
    }

    public virtual void LogDebug(string message)
    {
        if (MinimumLevel <= LogLevel.Debug)
        {
            Log(new LogEntry(LogLevel.Debug, message));
        }
    }

    public virtual void LogTrace(string message)
    {
        if (MinimumLevel <= LogLevel.Trace)
        {
            Log(new LogEntry(LogLevel.Trace, message));
        }
    }

    public virtual void LogCritical(string message, Exception? exception = null)
    {
        if (MinimumLevel <= LogLevel.Critical)
        {
            Log(new LogEntry(LogLevel.Critical, message, exception));
        }
    }
    public void LogError(string message, long? errorCode, int threadId, Exception? exception = null)
    {
        if (MinimumLevel <= LogLevel.Error)
        {
            Log(new LogEntry(LogLevel.Error, message, exception));
        }
    }

    protected abstract void Log(LogEntry entry);
}
