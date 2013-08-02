using System;

namespace DeltekReminder.DesktopApp.Pages
{
    public delegate void OnError(object sender, OnErrorArgs args);

    public class OnErrorArgs
    {
        public Exception Exception { get; set; }
    }
}