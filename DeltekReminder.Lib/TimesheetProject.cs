namespace DeltekReminder.Lib
{
    public class TimesheetProject
    {
        public int Row { get; set; }
        public string ProjectNumber { get; set; }
        public string ChargeDescription { get; set; }

        public override string ToString()
        {
            return "Row: " + Row;
        }
    }
}