using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace DeltekReminder.DesktopApp
{
    public partial class GenericBaloon
    {
        private string _message;

        public GenericBaloon()
        {
            InitializeComponent();
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                MessageText.Text = value; // todo: Implement INotifyPropertyChanged correctly
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
