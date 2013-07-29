using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using DeltekReminder.DesktopApp.Pages;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// This works for Deltek v9.0.1.0 build 9.0.1.144
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

        private DeltekPageBase[] pages;

        private void Login()
        {
            var loginPageUri = UrlUtils.GetLoginPage(_settings.BaseUrl);

            Browser.Navigate(loginPageUri);
            pages = new DeltekPageBase[]
                        {
                            new LoginPage(),
                            new HomePage(),
                            new TimesheetPage(),
                        };

            Browser.LoadCompleted += Browser_LoadCompleted;
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs args)
        {
            OnNavigatedToNewPage(triggeredByIframeRefresh: false);
        }

        private void OnNavigatedToNewPage(bool triggeredByIframeRefresh)
        {
            var document = (HTMLDocument)Browser.Document;
            var uri = new Uri(document.url);
            var currentPage = pages.FirstOrDefault(i => i.OnThisPage(_settings, uri, triggeredByIframeRefresh));
            if (currentPage != null)
            {
                if (currentPage is HomePage)
                {
                    SubscribeToOnActivate(document);
                }
                currentPage.DoStuff(_settings, Browser);
            }
        }

        private void SubscribeToOnActivate(HTMLDocument document)
        {
            var htmlDocument = document;
            DhtmlEventHandler myHandler = new DhtmlEventHandler(htmlDocument);
            myHandler.Handler += DocumentOnActivate;
            ((DispHTMLDocument) document).onactivate = myHandler;
        }

        public void DocumentOnActivate(IHTMLEventObj evo)
        {
            OnNavigatedToNewPage(triggeredByIframeRefresh: true);
        }
    }
}
