using System;
using System.Linq;
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
            var loginPageUri = UrlUtils.GetLoginPage(_settings.BaseUrl);

            Browser.Navigate(loginPageUri);
            int attemptedLogins = 0;
            Browser.LoadCompleted += (o, args) =>
                {
                    var onLoginPage = args.Uri == loginPageUri;
                    var onDesktopPage = UrlUtils.OnDesktopPage(args.Uri);
                    var loginFailed = attemptedLogins > 0;
                    if (onLoginPage && !loginFailed)
                    {
                        attemptedLogins++;
                        EnterCredentialsAndSubmit();
                    }
                    if (onDesktopPage)
                    {
                        var document = (HTMLDocument)Browser.Document;
                        var unitFrame = document.frames.item("unitFrame");
                        if (unitFrame == null)
                        {
                            throw new Exception("Unable to find unitFrame iframe");
                        }
                        HTMLDocument unitFrameDocument = (HTMLDocument)unitFrame.document;
                        var openTimesheet = unitFrameDocument.getElementsByTagName("tr")
                            .Cast<IHTMLElement>()
                            .Where(i => i.className == "notSelected")
                            .FirstOrDefault(i => i.innerText.Contains("Open"));

                        if (openTimesheet == null)
                        {
                            MessageBox.Show("No active timesheet");
                            return;
                        }
                        var allChildren = (IHTMLElementCollection)openTimesheet.all;
                        IHTMLElement spanToClick = allChildren
                            .Cast<IHTMLElement>()
                            .First(i => i.tagName.Equals("span", StringComparison.InvariantCultureIgnoreCase) && "desktopNormalAlertText".Equals(i.className, StringComparison.InvariantCultureIgnoreCase));
                        spanToClick.click();
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
