namespace ProductManager.CrossCuttingConcerns.DateTimes;

public interface IDateTimeProvider
{
    DateTime Now { get; }

    DateTime UtcNow { get; }

    DateTimeOffset OffsetNow { get; }

    DateTimeOffset OffsetUtcNow { get; }

    DateTime VietNameseTimeNow { get; }
}
