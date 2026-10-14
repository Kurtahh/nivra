namespace backend.Extensions;

using backend.Models;

public static class DateUtils
{
    public static DateOnly GetStartDate(TimePeriod period)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return period switch
        {
            TimePeriod.Daily => today,
            TimePeriod.Weekly => today.AddDays(-7),
            TimePeriod.Monthly => today.AddMonths(-1),
            TimePeriod.AllTime => DateOnly.MinValue,
            _ => today.AddDays(-7)
        };
    }
}