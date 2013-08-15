using System;
using System.Windows.Controls;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    public abstract class DeltekPageBase
    {
        public abstract bool OnThisPage(DeltekReminderContext settings, WebBrowser browser);

        public void TryGetTimesheet(DeltekReminderContext settings, WebBrowser browser)
        {
            try
            {
                TryGetTimesheetInternal(settings, browser);
            }
            catch (Exception ex)
            {
                InvokeOnError(ex);
            }
        }

        public abstract void TryGetTimesheetInternal(DeltekReminderContext settings, WebBrowser browser);

        public event OnError OnError;

        protected virtual void InvokeOnError(Exception ex)
        {
            var handler = OnError;
            if (handler != null) handler(this, new OnErrorArgs { Exception = ex });
        }

        protected static IHTMLWindow2 GetFrame(WebBrowser browser, string frameName)
        {
            var document = browser.Document as HTMLDocument;
            if (document == null) throw new Exception("Unable to find unitFrame iframe, document was null");
            return GetFrame(document, frameName);
        }

        protected static IHTMLWindow2 GetFrame(HTMLDocument document, string frameName)
        {
            for (int i = 0; i < document.frames.length; i++)
            {
                var frame = (IHTMLWindow2)document.frames.item(i);
                if (frame.name == frameName) return frame;
            }
            throw new Exception("Unable to find " + frameName + " iframe");
        }

        public IHTMLWindow2 GetUnitFrame(WebBrowser browser)
        {
            return GetFrame(browser, "unitFrame");
        }

        public HTMLDocument GetUnitFrameDocument(WebBrowser browser)
        {
            var unitFrame = GetUnitFrame(browser);
            if (unitFrame == null) return null;
            HTMLDocument unitFrameDocument = (HTMLDocument)unitFrame.document;
            return unitFrameDocument;
        }

        protected Uri GetUri(WebBrowser browser)
        {
            var document = (HTMLDocument)browser.Document;
            var uri = new Uri(document.url);
            return uri;
        }
    }
}
