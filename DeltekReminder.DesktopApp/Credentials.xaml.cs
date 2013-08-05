using System.Windows;
using DeltekReminder.Lib;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Credentials
    {
        private readonly DeltekReminderUiContext _ctx;

        public Credentials() : this(null)
        {
        }

        public Credentials(string message)
        {
            InitializeComponent();
            _ctx = DeltekReminderUiContext.GetInstance();
            PopulateControlValuesFromSettings();
            SetErrorMessage(message);
        }

        private void SetErrorMessage(string message)
        {
            ErrorMessage.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
            if (message != null)
                ErrorMessage.Text = message;
        }

        private void PopulateControlValuesFromSettings()
        {
            Username.Text = _ctx.Settings.Username;
            Password.Password = _ctx.Settings.Password;
            Domain.Text = _ctx.Settings.Domain;
            Url.Text = _ctx.Settings.BaseUrl;
        }

        private void OnConnect_Click(object sender, RoutedEventArgs e)
        {
            SaveControlValuesIntoSettings();
            _ctx.NavigationHelper.ShowStatusPage(NavigationService, showBrowser: false);
        }

        private void SaveControlValuesIntoSettings()
        {
            _ctx.Settings.Username = Username.Text;
            _ctx.Settings.Password = Password.Password;
            _ctx.Settings.Domain = Domain.Text;
            var uri = UrlUtils.GetBase(Url.Text);
            _ctx.Settings.BaseUrl = uri;
            _ctx.Settings.Save();
        }

    }
}
