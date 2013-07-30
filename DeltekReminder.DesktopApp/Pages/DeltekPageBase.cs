using System;
using System.Windows.Controls;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    public abstract class DeltekPageBase
    {
        public abstract bool OnThisPage(DeltekReminderContext settings, Uri uri, bool triggeredByIframeRefresh);

        public abstract void DoStuff(DeltekReminderContext settings, WebBrowser browser);

        public static IHTMLWindow2 GetUnitFrameGlobal(WebBrowser browser)
        {
            var document = (HTMLDocument)browser.Document;
            if (document.frames.length == 0) return null;
            var unitFrame = document.frames.item("unitFrame");
            if (unitFrame == null)
            {
                throw new Exception("Unable to find unitFrame iframe");
            }
            return (IHTMLWindow2)unitFrame;
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
