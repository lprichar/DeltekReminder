using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    public class HomePage : DeltekPageBase
    {
        public override bool OnThisPage(DeltekReminderSettings settings, Uri uri, bool triggeredByIframeRefresh)
        {
            return UrlUtils.OnTimeCollectionPage(uri) && !triggeredByIframeRefresh;
        }

        public override void DoStuff(DeltekReminderSettings settings, WebBrowser browser)
        {
            HTMLDocument unitFrameDocument = GetUnitFrameDocument(browser);
            var openTimesheet = unitFrameDocument.getElementsByTagName("tr")
                .Cast<IHTMLElement>()
                .Where(i => i.className == "notSelected")
                .FirstOrDefault(i => i.innerText.Contains("Open"));

            if (openTimesheet == null)
            {
                MessageBox.Show("No active timesheet");
                return;
            }
            var allChildren = (IHTMLElementCollection)openTimesheet.all;
            IHTMLElement spanToClick = allChildren
                .Cast<IHTMLElement>()
                .First(i => i.tagName.Equals("span", StringComparison.InvariantCultureIgnoreCase) && "desktopNormalAlertText".Equals(i.className, StringComparison.InvariantCultureIgnoreCase));

            spanToClick.click();
        }
    }
}
