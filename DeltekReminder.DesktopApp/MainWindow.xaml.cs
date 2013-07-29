using System;
using System.Linq;
using System.Windows;
using DeltekReminder.DesktopApp.Pages;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DeltekReminderSettings _settings;
        
        public MainWindow()
        {
            InitializeComponent();

            _settings = DeltekReminderSettings.GetSettings();

            var credentialsExist = !string.IsNullOrEmpty(_settings.BaseUrl);
            if (credentialsExist)
            {
                PopulateControlValuesFromSettings();
                Login();
            }
        }

        private void PopulateControlValuesFromSettings()
        {
            Username.Text = _settings.Username;
            Password.Password = _settings.Password;
            Domain.Text = _settings.Domain;
            Url.Text = _settings.BaseUrl;
        }

        private void OnConnect_Click(object sender, RoutedEventArgs e)
        {
            SaveControlValuesIntoSettings();
            Login();
        }

        private void SaveControlValuesIntoSettings()
        {
            _settings.Username = Username.Text;
            _settings.Password = Password.Password;
            _settings.Domain = Domain.Text;
            var uri = UrlUtils.GetBase(Url.Text);
            _settings.BaseUrl = uri;
            _settings.Save();
        }

        private void Login()
        {
            var loginPageUri = UrlUtils.GetLoginPage(_settings.BaseUrl);

            Browser.Navigate(loginPageUri);
            var pages = new DeltekPageBase[]
                        {
                            new LoginPage(),
                            new HomePage(),
                        };
            
            Browser.LoadCompleted += (o, args) =>
                {
                    var currentPage = pages.FirstOrDefault(i => i.OnThisPage(_settings, args.Uri));
                    if (currentPage != null)
                    {
                        currentPage.DoStuff(_settings, Browser);
                    }
                };
        }

    }
}
