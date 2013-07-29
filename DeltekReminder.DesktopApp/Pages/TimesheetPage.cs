using System;
using System.Windows;
using System.Windows.Controls;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    public class TimesheetPage : DeltekPageBase
    {
        public override bool OnThisPage(DeltekReminderSettings settings, Uri uri, bool triggeredByIframeRefresh)
        {
            return UrlUtils.OnTimeCollectionPage(uri) && triggeredByIframeRefresh;
        }

        public string GetTextAtCell(HTMLDocument document, int row, int column)
        {
            var id = string.Format("udt{0}_{1}", row, column);
            var projectNumberCell = document.getElementById(id);
            return projectNumberCell.innerText;
        }
        
        public override void DoStuff(DeltekReminderSettings settings, WebBrowser browser)
        {
            HTMLDocument unitFrameDocument = GetUnitFrameDocument(browser);

            Timesheet timesheet = new Timesheet();

            int row = 0;
            while (true)
            {
                var projectNumber = GetTextAtCell(unitFrameDocument, row, 0);
                var chargeDescription = GetTextAtCell(unitFrameDocument, row, 1);
                bool projectExistsAtRow = !string.IsNullOrEmpty(projectNumber);
                if (!projectExistsAtRow) break;

                var project = new TimesheetProject
                    {
                        RowNumber = row,
                        ProjectNumber = projectNumber,
                        ChargeDescription = chargeDescription,
                    };

                timesheet.Projects.Add(project);
                row++;
            }

            MessageBox.Show("Total Projects: " + timesheet.Projects.Count);
        }
    }
}
