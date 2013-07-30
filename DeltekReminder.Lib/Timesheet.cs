using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltekReminder.Lib
{
    public class Timesheet
    {
        public Timesheet()
        {
            Projects = new List<TimesheetProject>();
            Entries = new List<TimesheetEntry>();
            Days = new List<TimesheetDay>();
        }

        public IList<TimesheetProject> Projects { get; set; }
        
        public IList<TimesheetEntry> Entries { get; set; }

        public IList<TimesheetDay> Days { get; set; }
        
        public DateTime EndingDate { get; set; }

        public int DaysInPeriod { get; set; }

        public DateTime StartingDate { get; set; }

        public bool IsMissingTimeForToday(DeltekReminderContext ctx)
        {
            if (ctx.Now.DayOfWeek == DayOfWeek.Saturday || ctx.Now.DayOfWeek == DayOfWeek.Sunday) return false;
            return !ContainsAnyTimeOnDay(ctx.Now);
        }

        private bool ContainsAnyTimeOnDay(DateTime date)
        {
            return Entries.Any(i => SameDay(i.Date, date) && i.Hours > 0);
        }

        private bool SameDay(DateTime d1, DateTime d2)
        {
            return d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day;
        }
    }
}