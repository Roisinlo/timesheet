namespace Timesheet.Core.Helpers;

public static class WeekHelper
{
    /// <summary>
    /// Returns the Monday of the week containing the given date.
    /// </summary>
    public static DateTime GetStartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }
}