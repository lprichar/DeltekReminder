using System;

namespace DeltekReminder.Lib.Utils
{
    public static class DateUtils
    {
        public static DateTime ParsePeriodEndingDate(string periodEnding)
        {
            return DateTime.Parse(periodEnding);
        }

        public static bool SameDay(this DateTime d1, DateTime d2)
        {
            return d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day;
        }

    }
}
