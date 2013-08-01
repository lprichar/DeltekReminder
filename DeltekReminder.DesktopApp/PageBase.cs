using System.Windows.Controls;
using System.Windows.Navigation;

namespace DeltekReminder.DesktopApp
{
    public class PageBase : Page
    {
        protected void NavigateToStatusPage(DeltekReminderUiContext ctx)
        {
            ((NavigationWindow)Parent).Source = ctx.NavigationHelper.StatusPage;
        }

        protected void NavigateToCredentialsPage(DeltekReminderUiContext ctx)
        {
            ((NavigationWindow)Parent).Source = ctx.NavigationHelper.CredentialsPage;
        }
    }
}
