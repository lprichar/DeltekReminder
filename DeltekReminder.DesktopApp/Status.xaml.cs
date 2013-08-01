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
    /// Interaction logic for Status.xaml
    /// </summary>
    public partial class Status
    {
        private readonly DeltekReminderUiContext _ctx;

        public Status()
        {
            InitializeComponent();
            Browser.LoadCompleted += Browser_LoadCompleted;
            _ctx = DeltekReminderUiContext.GetInstance();
            
            if (_ctx.Settings.LastSuccessfulDeltekCheck.HasValue)
            {
                LastSuccessfulDeltekCheck.Text = _ctx.Settings.LastSuccessfulDeltekCheck.Value.ToLongDateString();
            }
            Login();
        }

        private void Login()
        {
            var loginPageUri = UrlUtils.GetLoginPage(_ctx.Settings.BaseUrl);

            var timesheetPage = new TimesheetPage();
            var loginPage = new LoginPage();
            timesheetPage.FoundTimesheet += TimesheetPageFoundTimesheet;
            loginPage.FailedLogin += LoginPageFailedLogin;
            _pages = new DeltekPageBase[]
                        {
                            loginPage,
                            new HomePage(),
                            timesheetPage
                        };
            Browser.Navigate(loginPageUri);
        }

        private void LoginPageFailedLogin(object sender, FailedLoginArgs args)
        {
            NavigateToCredentialsPage(_ctx);
        }

        private DeltekPageBase[] _pages;

        private void TimesheetPageFoundTimesheet(object sender, FoundTimesheetArgs args)
        {
            // todo: Set _ctx.LastSuccessfulDeltekCheck = _ctx.Now and Save();
            
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
