using System;

namespace DeltekReminder.Lib
{
    public class TimesheetEntry
    {
        public int Row { get; set; }
        
        public DateTime Date { get; set; }
        
        public double Hours { get; set; }

        public int Column { get; set; }
    }
}