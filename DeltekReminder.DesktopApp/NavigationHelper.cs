using System;
using System.Reflection;
using System.Windows.Navigation;

namespace DeltekReminder.DesktopApp
{
    public class NavigationHelper
    {
        public void ShowStatusPage(NavigationService navigationService, bool showBrowser)
        {
            var statusPage = ShowStatusPage(navigationService);
            statusPage.ShowBrowser(showBrowser);
        }

        private static Status ShowStatusPage(NavigationService navigationService)
        {
            var statusPage = navigationService.Content as Status;
            if (statusPage == null)
            {
                statusPage = new Status();
                navigationService.Navigate(statusPage);
            }
            return statusPage;
        }

        public void ShowCredentialsPage(NavigationService navigationService, string message = null)
        {
            var currentPage = navigationService.Content as Credentials;
            if (currentPage == null)
            {
                currentPage = new Credentials(message);
                navigationService.Navigate(currentPage);
            }
        }

        public void ShowStatusPage(NavigationService navigationService, SetHoursForTodayArgs setHoursForTodayArgs)
        {
            var statusPage = ShowStatusPage(navigationService);
            statusPage.SetHoursForToday(setHoursForTodayArgs);
        }

        public Version GetVersion()
        {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = executingAssembly.GetName();
            return assemblyName.Version;
        }
    }
}
