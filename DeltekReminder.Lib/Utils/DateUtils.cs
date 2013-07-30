using System;

namespace DeltekReminder.Lib.Utils
{
    public class DateUtils
    {
        public static DateTime ParsePeriodEndingDate(string periodEnding)
        {
            return DateTime.Parse(periodEnding);
        }
    }
}
