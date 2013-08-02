using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
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

            if (_ctx.Settings.LastSuccessfulLogin.HasValue)
            {
                EnsureTimerExists();
            }
            else
            {
                Login();
            }
            Databind();
        }

        private void EnsureTimerExists()
        {
            _ctx.SchedulerService.EnsureTimerExists(_ctx, OnTimeToCheckDeltek);
            Databind();
        }

        private void Databind()
        {
            LastSuccessfulDeltekCheck.Text = _ctx.Settings.GetLastStatusAsText();
            NextCheck.Text = _ctx.SchedulerService.GetNextTimeToCheckAsText();
        }

        private void OnTimeToCheckDeltek(object state)
        {
            Dispatcher.BeginInvoke(new Action(Login), DispatcherPriority.Normal, null);
        }

        private void SuccessfulLogin(object sender, SuccessfulLoginArgs args)
        {
            EnsureTimerExists();
            _ctx.Settings.LastSuccessfulLogin = _ctx.Now;
            _ctx.Settings.Save();
            var thisIsTheFirstSuccessfulLogin = !_ctx.Settings.LastSuccessfulLogin.HasValue;
            if (thisIsTheFirstSuccessfulLogin)
            {
                args.Cancel = true;
            }
            Databind();
        }

        private void Login()
        {
            var loginPageUri = UrlUtils.GetLoginPage(_ctx.Settings.BaseUrl);

            var timesheetPage = new TimesheetPage();
            var loginPage = new LoginPage();
            var homePage = new HomePage();
            timesheetPage.FoundTimesheet += TimesheetPageFoundTimesheet;
            loginPage.FailedLogin += LoginPageFailedLogin;
            homePage.SuccessfulLogin += SuccessfulLogin;
            _pages = new DeltekPageBase[]
                        {
                            loginPage,
                            homePage,
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
