using System.Collections.Generic;

namespace DeltekReminder.Lib
{
    public class Timesheet
    {
        public Timesheet()
        {
            Projects = new List<TimesheetProject>();
            Entries = new List<TimesheetEntry>();
        }

        public IList<TimesheetProject> Projects { get; set; }
        public IList<TimesheetEntry> Entries { get; set; }
    }
}