using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Navigation;
using DeltekReminder.DesktopApp.Pages;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Credentials
    {
        private readonly DeltekReminderContext _ctx;

        public Credentials()
        {
            InitializeComponent();
            try
            {
                _ctx = new DeltekReminderContext();
            }
            catch (SerializationException ex)
            {
                var dialogResult = MessageBox.Show("There was an error deserializing the settings file.  Click OK to revert the file and start over or cancel to close the app and fix the problem yourself.  Here's the error: " + ex, "Drat!", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.Cancel)
                {
                    Application.Current.Shutdown();
                }
            }
            var credentialsExist = !string.IsNullOrEmpty(_ctx.Settings.BaseUrl);
            if (credentialsExist)
            {
                PopulateControlValuesFromSettings();
                Login();
            }
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
            Login();
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

        private DeltekPageBase[] _pages;

        private void Login()
        {
            var loginPageUri = UrlUtils.GetLoginPage(_ctx.Settings.BaseUrl);

            Browser.LoadCompleted += Browser_LoadCompleted;
            var timesheetPage = new TimesheetPage();
            timesheetPage.FoundTimesheet += TimesheetPageFoundTimesheet;
            _pages = new DeltekPageBase[]
                        {
                            new LoginPage(),
                            new HomePage(),
                            timesheetPage
                        };
            Browser.Navigate(loginPageUri);
        }

        private void TimesheetPageFoundTimesheet(object sender, FoundTimesheetArgs args)
        {
            if (args.Timesheet.IsMissingTimeForToday(_ctx))
            {
                MessageBox.Show("Missing timesheet for today!");
            }
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs args)
        {
            OnNavigatedToNewPage(triggeredByIframeRefresh: false);
        }

        private void OnNavigatedToNewPage(bool triggeredByIframeRefresh)
        {
            var document = (HTMLDocument)Browser.Document;
            var uri = new Uri(document.url);
            var currentPage = _pages.FirstOrDefault(i => i.OnThisPage(_ctx, uri, triggeredByIframeRefresh));
            if (currentPage != null)
            {
                if (currentPage is HomePage)
                {
                    SubscribeToOnActivate(document);
                }
                currentPage.TryGetTimesheet(_ctx, Browser);
            }
        }

        private void SubscribeToOnActivate(HTMLDocument document)
        {
            var htmlDocument = document;
            DhtmlEventHandler myHandler = new DhtmlEventHandler(htmlDocument);
            myHandler.Handler += DocumentOnActivate;
            ((DispHTMLDocument)document).onactivate = myHandler;
        }

        public void DocumentOnActivate(IHTMLEventObj evo)
        {
            OnNavigatedToNewPage(triggeredByIframeRefresh: true);
        }
    }
}
