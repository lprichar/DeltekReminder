using System;
using System.Windows;
using mshtml;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private DeltekReminderSettings _settings;
        
        public MainWindow()
        {
            InitializeComponent();

            _settings = DeltekReminderSettings.GetSettings();

            if (!string.IsNullOrEmpty(_settings.Url))
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
            Url.Text = _settings.Url;
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
            _settings.Url = Url.Text;
            _settings.Save();
        }

        private void Login()
        {
            var source = new Uri(_settings.Url);
            Browser.Navigate(source);
            Browser.LoadCompleted += (o, args) =>
                {
                    if (args.Uri == source)
                    {
                        var document = (HTMLDocument)Browser.Document;
                        SetValueById(document, "uid", _settings.Username);
                        SetValueById(document, "passField", _settings.Password);
                        SetValueById(document, "dom", _settings.Domain);
                    }
                };
        }

        private void SetValueById(HTMLDocument document, string id, string value)
        {
            IHTMLElement element = document.getElementById(id);
            if (element == null) return;
            element.setAttribute("value", value);
        }
    }
}
