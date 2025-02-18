namespace ProductManager.Infrastructure.DateTimes;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset OffsetNow => DateTimeOffset.Now;

    public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
    public DateTimeOffset VietNameseTimeNow => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "SE Asia Standard Time");
}
