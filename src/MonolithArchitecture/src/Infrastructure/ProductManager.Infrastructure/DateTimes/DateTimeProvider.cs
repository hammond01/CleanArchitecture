namespace ProductManager.Infrastructure.DateTimes;

public class DateTimeProvider : IDateTimeProvider
{

    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTimeOffset OffsetNow => DateTimeOffset.Now;

    public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
    public DateTime VietNameseTimeNow => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "SE Asia Standard Time");
}
