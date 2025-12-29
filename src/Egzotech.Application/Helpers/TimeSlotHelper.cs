namespace Egzotech.Application.Helpers;

public static class TimeSlotHelper
{
    public static bool HasValidMinutes(DateTimeOffset date)
    {
        return date.Minute == 0 || date.Minute == 30;
    }

    public static bool HasZeroSeconds(DateTimeOffset date)
    {
        return date.Second == 0 && date.Millisecond == 0;
    }
}