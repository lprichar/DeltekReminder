using System.Runtime.Serialization;
using System.Windows;
using DeltekReminder.Lib;

namespace DeltekReminder.DesktopApp
{
    public class DeltekReminderUiContext : DeltekReminderContext
    {
        public static DeltekReminderUiContext GetInstance()
        {
            try
            {
                return new DeltekReminderUiContext();
            }
            catch (SerializationException ex)
            {
                MessageBox.Show("There was an error deserializing the settings file.  Here's the error: " + ex, "Drat!");
                Application.Current.Shutdown();
                return null;
            }
        }

        NavigationHelper _navigationHelper = new NavigationHelper();

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper ?? (_navigationHelper = new NavigationHelper()); }
        }
    }
}
