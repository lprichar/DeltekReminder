using System;
using System.Windows.Controls;

namespace DeltekReminder.DesktopApp
{
    public abstract class DeltekPageBase
    {
        public abstract bool OnThisPage(DeltekReminderSettings settings, Uri uri);

        public abstract void DoStuff(DeltekReminderSettings settings, WebBrowser browser);
    }
}
