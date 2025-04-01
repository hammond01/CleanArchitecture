namespace ProductManager.Shared.Logging;

public class ConsoleLogger : LoggerBase
{
    public ConsoleLogger(LogLevel minimumLevel = LogLevel.Information)
        : base(minimumLevel)
    {
    }

    protected override void Log(LogEntry entry)
    {
        Console.WriteLine($"[{entry.Level}] {entry.Message}");
        if (entry.Exception != null)
        {
            Console.WriteLine($"Exception: {entry.Exception}");
        }
    }
}
