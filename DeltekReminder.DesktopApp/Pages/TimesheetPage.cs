using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using DeltekReminder.Lib;
using DeltekReminder.Lib.Utils;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    public class TimesheetPage : DeltekPageBase
    {
        public event FoundTimesheet FoundTimesheet;

        protected virtual void InvokeFoundTimesheet(Timesheet timesheet)
        {
            var handler = FoundTimesheet;
            if (handler != null) handler(this, new FoundTimesheetArgs { Timesheet = timesheet });
        }

        public override bool OnThisPage(DeltekReminderContext ctx, WebBrowser browser)
        {
            var uri = GetUri(browser);
            if (!UrlUtils.OnTimeCollectionPage(uri)) return false;

            HTMLDocument unitFrameDocument = GetUnitFrameDocument(browser);
            if (unitFrameDocument == null) return false;

            var endingDateText = unitFrameDocument.getElementById("endingDateSpan");
            return endingDateText != null;
        }

        public string GetTextAtCell(HTMLDocument document, int row, int column)
        {
            var id = string.Format("udt{0}_{1}", row, column);
            var projectNumberCell = document.getElementById(id);
            return projectNumberCell.innerText;
        }

        public void SetHoursAtCell(HTMLDocument document, int row, int column, decimal val)
        {
            var hoursCell = GetHoursCell(document, row, column);
            hoursCell.click();
            var textBox = (HTMLInputElement)hoursCell.children[0];
            textBox.value = val.ToString(CultureInfo.InvariantCulture);
        }
        
        public decimal? GetHoursAtCell(HTMLDocument document, int row, int column)
        {
            var hoursCell = GetHoursCell(document, row, column);
            if (hoursCell == null) return null;
            var text = hoursCell.innerText;
            if (string.IsNullOrEmpty(text)) return null;
            return decimal.Parse(text);
        }

        private static IHTMLElement GetHoursCell(HTMLDocument document, int row, int column)
        {
            var id = string.Format("hrs{0}_{1}", row, column);
            var hoursCell = document.getElementById(id);
            return hoursCell;
        }

        public override void TryGetTimesheetInternal(DeltekReminderContext ctx, WebBrowser browser)
        {
            HTMLDocument unitFrameDocument = GetUnitFrameDocument(browser);
            var timesheet = GetTimesheetFromDocument(unitFrameDocument);
            InvokeFoundTimesheet(timesheet);
        }

        private Timesheet GetTimesheetFromDocument(HTMLDocument unitFrameDocument)
        {
            Timesheet timesheet = new Timesheet();

            PopulateTimesheetAttributes(unitFrameDocument, timesheet);
            PopulateDays(timesheet);
            PopulateProjects(unitFrameDocument, timesheet);
            PopulateEntries(unitFrameDocument, timesheet);

            return timesheet;
        }

        private void PopulateDays(Timesheet timesheet)
        {
            for (var column = 0; column < timesheet.DaysInPeriod; column++)
            {
                var timesheetDay = new TimesheetDay
                    {
                        Column = column,
                        Date = timesheet.StartingDate.AddDays(column)
                    };

                timesheet.Days.Add(timesheetDay);
            }
        }

        private void PopulateTimesheetAttributes(HTMLDocument document, Timesheet timesheet)
        {
            var endingDateText = document.getElementById("endingDateSpan").innerText;
            var endingDate = DateUtils.ParsePeriodEndingDate(endingDateText);

            var daysInPeriod = document.getElementsByTagName("div")
                                       .Cast<IHTMLElement>()
                                       .Count(i => i.className == "hrsHeaderText" && !string.IsNullOrEmpty(i.innerText));

            timesheet.EndingDate = endingDate;
            timesheet.DaysInPeriod = daysInPeriod;
            timesheet.StartingDate = endingDate.AddDays(-daysInPeriod+1);
        }

        private void PopulateEntries(HTMLDocument unitFrameDocument, Timesheet timesheet)
        {
            foreach (var project in timesheet.Projects)
            {
                foreach (var day in timesheet.Days)
                {
                    var hours = GetHoursAtCell(unitFrameDocument, project.Row, day.Column);
                    if (hours == null) continue;
                    var timesheetEntry = new TimesheetEntry
                    {
                        Row = project.Row,
                        Column = day.Column,
                        Date = day.Date,
                        Hours = hours.Value,
                    };
                    timesheet.Entries.Add(timesheetEntry);
                }
            }
        }

        private void PopulateProjects(HTMLDocument unitFrameDocument, Timesheet timesheet)
        {
            int row = 0;
            while (true)
            {
                var projectNumber = GetTextAtCell(unitFrameDocument, row, 0);
                var chargeDescription = GetTextAtCell(unitFrameDocument, row, 1);
                bool projectExistsAtRow = !string.IsNullOrEmpty(projectNumber);
                if (!projectExistsAtRow) break;

                var project = new TimesheetProject
                    {
                        Row = row,
                        ProjectNumber = projectNumber,
                        ChargeDescription = chargeDescription,
                    };

                timesheet.Projects.Add(project);
                row++;
            }
        }

        public void SetHoursForToday(DeltekReminderContext ctx, WebBrowser browser, decimal hours)
        {
            HTMLDocument unitFrameDocument = GetUnitFrameDocument(browser);
            var timesheet = GetTimesheetFromDocument(unitFrameDocument);
            var projectToSet = timesheet.GetProjectWithMostHours();
            var todayDay = timesheet.GetTodayDay(ctx);
            SetHoursAtCell(unitFrameDocument, projectToSet.Row, todayDay.Column, hours);

            // todo: Save()
        }
    }

    public delegate void FoundTimesheet(object sender, FoundTimesheetArgs args);

    public class FoundTimesheetArgs
    {
        public Timesheet Timesheet { get; set; }
    }
}
