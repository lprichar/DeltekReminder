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
            var dependencyObject = Parent;
            var parent = (MainWindow)dependencyObject;
            parent.SetStatus(statusText);
        }
    }

}
