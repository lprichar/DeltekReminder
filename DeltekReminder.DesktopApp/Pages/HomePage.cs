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
        public override bool OnThisPage(DeltekReminderSettings settings, Uri uri)
        {
            return UrlUtils.OnDesktopPage(uri);
        }

        public override void DoStuff(DeltekReminderSettings settings, WebBrowser browser)
        {
            var document = (HTMLDocument)browser.Document;
            var unitFrame = document.frames.item("unitFrame");
            if (unitFrame == null)
            {
                throw new Exception("Unable to find unitFrame iframe");
            }
            HTMLDocument unitFrameDocument = (HTMLDocument)unitFrame.document;
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
