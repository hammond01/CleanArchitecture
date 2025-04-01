namespace ProductManager.Shared.Logging;

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public Exception? Exception { get; set; }
    public string? Category { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }

    public LogEntry(LogLevel level, string message, Exception? exception = null)
    {
        Timestamp = DateTime.UtcNow;
        Level = level;
        Message = message;
        Exception = exception;
        AdditionalData = new Dictionary<string, object>();
    }

    public void AddData(string key, object value)
    {
        AdditionalData ??= new Dictionary<string, object>();
        AdditionalData[key] = value;
    }
}
