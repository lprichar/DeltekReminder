using System;

namespace DeltekReminder.DesktopApp
{
    public class NavigationHelper
    {
        public Uri StatusPage
        {
            get { return new Uri("Status.xaml", UriKind.Relative); }
        }

        public Uri CredentialsPage
        {
            get { return new Uri("Credentials.xaml", UriKind.Relative); }
        }
    }
}
