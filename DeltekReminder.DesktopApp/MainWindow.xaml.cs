using System;
using System.Windows;
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
            var loginPageUri = new Uri(_settings.BaseUrl + "/DeltekTC/welcome.msv");

            Browser.Navigate(loginPageUri);
            int numberOfTimesLoginPage = 0;
            Browser.LoadCompleted += (o, args) =>
                {
                    var onLoginPage = args.Uri == loginPageUri;
                    var loginFailed = numberOfTimesLoginPage > 0;
                    if (onLoginPage && !loginFailed)
                    {
                        numberOfTimesLoginPage++;
                        EnterCredentialsAndSubmit();
                    }
                };
        }

        private void EnterCredentialsAndSubmit()
        {
            var document = (HTMLDocument) Browser.Document;
            SetValueById(document, "uid", _settings.Username);
            SetValueById(document, "passField", _settings.Password);
            SetValueById(document, "dom", _settings.Domain);

            IHTMLElement loginButton = document.getElementById("loginButton");
            loginButton.click();
        }

        private void SetValueById(HTMLDocument document, string id, string value)
        {
            IHTMLElement element = document.getElementById(id);
            if (element == null) return;
            element.setAttribute("value", value);
        }
    }
}
