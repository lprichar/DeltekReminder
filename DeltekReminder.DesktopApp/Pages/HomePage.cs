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
        public event SuccessfulLogin SuccessfulLogin;

        protected virtual void OnSuccessfulLogin(SuccessfulLoginArgs args)
        {
            var handler = SuccessfulLogin;
            if (handler != null) handler(this, args);
        }

        public override bool OnThisPage(DeltekReminderContext ctx, Uri uri, bool triggeredByIframeRefresh)
        {
            return UrlUtils.OnTimeCollectionPage(uri) && !triggeredByIframeRefresh;
        }

        public override void TryGetTimesheet(DeltekReminderContext ctx, WebBrowser browser)
        {
            // if we got to this page it must have been a successful login, because this is where deltek sends you on login success
            var args = new SuccessfulLoginArgs();
            OnSuccessfulLogin(args);
            if (args.Cancel) return;
            
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
