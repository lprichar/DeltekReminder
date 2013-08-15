using System;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    internal class WebpageException : Exception
    {
        public WebpageException(string message, HTMLDocument document)
            : base(message)
        {
            Url = document.url;
            Html = document.body.innerHTML;
        }

        public string Html { get; set; }

        public string Url { get; set; }
    }
}