using System.Windows.Controls;

namespace DeltekReminder.DesktopApp
{
    public class PageBase : Page
    {
        protected void NavigateToCredentialsPage(DeltekReminderUiContext ctx, string message)
        {
            ctx.NavigationHelper.ShowCredentialsPage(NavigationService, message);
        }

        protected void SetStatus(string statusText)
        {
            var parent = GetMainWindow();
            if (parent == null) return;
            parent.SetStatus(statusText);
        }

        protected bool IsPerformingAsyncOperation()
        {
            var parent = GetMainWindow();
            if (parent == null) return false;
            return parent.IsLoadingAnimationVisible();
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
