﻿using System;
using System.Reflection;
using System.Windows.Navigation;

namespace DeltekReminder.DesktopApp
{
    public class NavigationHelper
    {
        public void ShowStatusPage(NavigationService navigationService, bool showBrowser)
        {
            var statusPage = navigationService.Content as Status;
            if (statusPage == null)
            {
                statusPage = new Status();
                navigationService.Navigate(statusPage);
            }
            statusPage.ShowBrowser(showBrowser);
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
