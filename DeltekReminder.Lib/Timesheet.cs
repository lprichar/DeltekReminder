using System;
using System.Collections.Generic;

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
    }
}