namespace ProductManager.Shared.DateTimes;

public interface IDateTimeProvider
{
    DateTimeOffset OffsetNow { get; }

    DateTimeOffset OffsetUtcNow { get; }

    DateTimeOffset VietNameseTimeNow { get; }
}
