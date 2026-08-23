namespace Nutra.Helper;

public static class DateTimeHelper
{
    public static DateTime EnsureUtcDateTime(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        
        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime.ToUniversalTime();
        
        return dateTime;
    }
}
