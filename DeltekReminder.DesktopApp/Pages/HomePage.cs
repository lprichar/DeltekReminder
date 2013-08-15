using System;
using System.Linq;
using System.Windows.Controls;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    public class HomePage : DeltekPageBase
    {
        public event SuccessfulLogin SuccessfulLogin;
        public event NoActiveTimesheet NoActiveTimesheet;

        protected virtual void InvokeNoActiveTimesheet()
        {
            var handler = NoActiveTimesheet;
            if (handler != null) handler(this, new NoActiveTimesheetArgs());
        }

        protected virtual void InvokeSuccessfulLogin(SuccessfulLoginArgs args)
        {
            var handler = SuccessfulLogin;
            if (handler != null) handler(this, args);
        }

        public override bool OnThisPage(DeltekReminderContext ctx, WebBrowser browser)
        {
            var uri = GetUri(browser);
            if (!UrlUtils.OnTimeCollectionPage(uri)) return false;

            HTMLDocument unitFrameDocument = GetUnitFrameDocument(browser);
            if (unitFrameDocument == null) return false;

            var myTimesheetsOnPage = unitFrameDocument.body.innerText.Contains("MyTimesheets");
            return myTimesheetsOnPage;
        }

        public override void TryGetTimesheetInternal(DeltekReminderContext ctx, WebBrowser browser)
        {
            // if we got to this page it must have been a successful login, because this is where deltek sends you on login success
            var args = new SuccessfulLoginArgs();
            InvokeSuccessfulLogin(args);
            if (args.Cancel) return;
            
            HTMLDocument unitFrameDocument = GetUnitFrameDocument(browser);
            var openTimesheet = unitFrameDocument.getElementsByTagName("tr")
                .Cast<IHTMLElement>()
                .Where(i => i.className == "notSelected")
                .LastOrDefault(i => i.innerText.Contains("Open")); // note: LastOrDefault implicitly assumes open timesheets are in reverse chronological order

            if (openTimesheet == null)
            {
                InvokeNoActiveTimesheet();
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
