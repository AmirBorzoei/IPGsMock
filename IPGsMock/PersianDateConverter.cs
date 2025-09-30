using System.Globalization;

namespace IPGsMock;

public static class PersianDateConverter
{
    private static readonly PersianCalendar _persianCalendar = new();

    public static DateTime ToPersianDate(DateTimeOffset dateTimeOffset)
    {
        if (dateTimeOffset == DateTimeOffset.MinValue)
        {
            return DateTime.MinValue;
        }

        var dateTime = dateTimeOffset.DateTime;
        int year = _persianCalendar.GetYear(dateTime);
        int month = _persianCalendar.GetMonth(dateTime);
        int day = _persianCalendar.GetDayOfMonth(dateTime);

        return new DateTime(year, month, day, _persianCalendar);
    }

    public static string ToPersianDateString(DateTimeOffset dateTimeOffset)
    {
        if (dateTimeOffset == DateTimeOffset.MinValue)
        {
            return "0000/00/00 00:00";
        }

        var dateTime = dateTimeOffset.DateTime;
        int year = _persianCalendar.GetYear(dateTime);
        int month = _persianCalendar.GetMonth(dateTime);
        int day = _persianCalendar.GetDayOfMonth(dateTime);

        return $"{year:0000}/{month:00}/{day:00} - {dateTime.Hour}:{dateTime.Minute}";
    }
}