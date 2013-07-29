using System.Runtime.InteropServices;
using mshtml;

namespace DeltekReminder.DesktopApp
{
    public delegate void DhtmlEvent(IHTMLEventObj e);

    /// <summary>
    /// Events are tricky with WebBrowser in WPF:
    /// http://social.msdn.microsoft.com/Forums/ie/en-US/1ea154a5-5802-460c-9941-30f14b36d0a4/mouse-event-handling-problem-in-bho
    /// http://www.west-wind.com/weblog/posts/2004/Apr/27/Handling-mshtml-Document-Events-without-Mouse-lockups
    /// </summary>
    [ComVisible(true)]
    public class DhtmlEventHandler
    {
        public DhtmlEvent Handler;
        readonly HTMLDocument _document;
        public DhtmlEventHandler(HTMLDocument doc)
        {
            _document = doc;
        }

        [DispId(0)]
        public void Call()
        {
            Handler(_document.parentWindow.@event);
        }
    }
}