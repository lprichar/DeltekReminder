using System;
using System.Windows.Controls;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    public abstract class DeltekPageBase
    {
        public abstract bool OnThisPage(DeltekReminderContext settings, Uri uri, bool triggeredByIframeRefresh);

        public abstract void TryGetTimesheet(DeltekReminderContext settings, WebBrowser browser);

        public static IHTMLWindow2 GetUnitFrameGlobal(WebBrowser browser)
        {
            var document = browser.Document as HTMLDocument;
            if (document.frames.length == 0) return null;
            var unitFrame = document.frames.item(1) as IHTMLWindow2;
            if (unitFrame == null || unitFrame.name != "unitFrame")
            {
                throw new Exception("Unable to find unitFrame iframe");
            }
            return unitFrame;
        }

        public IHTMLWindow2 GetUnitFrame(WebBrowser browser)
        {
            return GetUnitFrameGlobal(browser);
        }

        public HTMLDocument GetUnitFrameDocument(WebBrowser browser)
        {
            var unitFrame = GetUnitFrame(browser);
            if (unitFrame == null) return null;
            HTMLDocument unitFrameDocument = (HTMLDocument)unitFrame.document;
            return unitFrameDocument;
        }
    }
}
