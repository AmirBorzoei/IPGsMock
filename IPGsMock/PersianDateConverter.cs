using System.Globalization;

namespace IPGsMock;

public static class PersianDateConverter
{
    private static readonly PersianCalendar _persianCalendar = new();

    public static DateTime ToPersianDate(DateTimeOffset dateTimeOffset)
    {
        var dateTime = dateTimeOffset.DateTime;
        int year = _persianCalendar.GetYear(dateTime);
        int month = _persianCalendar.GetMonth(dateTime);
        int day = _persianCalendar.GetDayOfMonth(dateTime);

        return new DateTime(year, month, day, _persianCalendar);
    }

    public static string ToPersianDateString(DateTimeOffset dateTimeOffset)
    {
        var dateTime = dateTimeOffset.DateTime;
        int year = _persianCalendar.GetYear(dateTime);
        int month = _persianCalendar.GetMonth(dateTime);
        int day = _persianCalendar.GetDayOfMonth(dateTime);

        return $"{year:0000}/{month:00}/{day:00} - {dateTime.Hour}:{dateTime.Minute}";
    }
}