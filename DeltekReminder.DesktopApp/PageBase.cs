using System;
using System.Windows.Controls;

namespace DeltekReminder.DesktopApp
{
    public class PageBase : Page
    {
        protected void NavigateToStatusPage(DeltekReminderUiContext ctx)
        {
            if (NavigationService == null) throw new NullReferenceException("NavigationService is null");
            NavigationService.Navigate(ctx.NavigationHelper.StatusPage);
        }

        protected void NavigateToCredentialsPage(DeltekReminderUiContext ctx)
        {
            if (NavigationService == null) throw new NullReferenceException("NavigationService is null");
            NavigationService.Navigate(ctx.NavigationHelper.CredentialsPage);
        }

        protected void SetStatus(string statusText)
        {
            var parent = GetMainWindow();
            parent.SetStatus(statusText);
        }

        private MainWindow GetMainWindow()
        {
            return Parent as MainWindow;
        }

        protected void SetTrayAlert(string message)
        {
            var parent = GetMainWindow();
            parent.SetTrayAlert(message);
        }
    }

}
