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
        private readonly StatusViewModel _statusViewModel;

        public Status()
        {
            InitializeComponent();
            Browser.LoadCompleted += Browser_LoadCompleted;
            _ctx = DeltekReminderUiContext.GetInstance();
            
            _statusViewModel = new StatusViewModel();
            DataContext = _statusViewModel;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var isLoggingInForFirstTime = !_ctx.Settings.LastSuccessfulLogin.HasValue;
            if (isLoggingInForFirstTime)
            {
                // login needs to occur in OnRender because it calls SetStatus which requires that the parent NavigationWindow's template has been rendered
                Login();
            }
            else
            {
                EnsureTimerExists();
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
            _statusViewModel.NextCheckDay = _ctx.SchedulerService.GetNextDayToCheckAsText();
        }

        private void OnTimeToCheckDeltek(object state)
        {
            Dispatcher.BeginInvoke(new Action(Login), DispatcherPriority.Normal, null);
        }

        private void SuccessfulLogin(object sender, SuccessfulLoginArgs args)
        {
            _ctx.Settings.LastSuccessfulLogin = _ctx.Now;
            _ctx.Settings.Save();
            var thisIsTheFirstSuccessfulLogin = !_ctx.Settings.LastSuccessfulLogin.HasValue;
            if (thisIsTheFirstSuccessfulLogin)
            {
                EnsureTimerExists();
                args.Cancel = true;
                SetStatus(null);
            }
            else
            {
                SetStatus("Getting active timesheet...");
            }
            Databind();
        }

        private void Login()
        {
            SetStatus("Logging in...");
            var loginPageUri = UrlUtils.GetLoginPage(_ctx.Settings.BaseUrl);

            var timesheetPage = new TimesheetPage();
            var loginPage = new LoginPage();
            var homePage = new HomePage();
            timesheetPage.FoundTimesheet += TimesheetPageFoundTimesheet;
            loginPage.FailedLogin += LoginPageFailedLogin;
            homePage.SuccessfulLogin += SuccessfulLogin;
            homePage.NoActiveTimesheet += OnNoActiveTimesheet;
            _pages = new DeltekPageBase[]
                        {
                            loginPage,
                            homePage,
                            timesheetPage
                        };
            foreach (var page in _pages)
            {
                page.OnError += OnGetTimesheetError;
            }
            Browser.Navigate(loginPageUri);
        }

        private void OnGetTimesheetError(object sender, OnErrorArgs args)
        {
            SetStatus(null);
            var hasEverLoggedInSuccessfully = _ctx.Settings.LastSuccessfulLogin.HasValue;
            if (hasEverLoggedInSuccessfully)
            {
                MessageBox.Show("Error occurred retrieving timesheet.  Error: " + args.Exception.Message);
            }
            else
            {
                NavigateToCredentialsPage(_ctx, "Unexpected error occurred loggin in: " + args.Exception.Message);
            }
        }

        private void OnNoActiveTimesheet(object sender, NoActiveTimesheetArgs args)
        {
            SetStatus(null);
            SetTrayAlert("No active timesheet");
        }

        private void LoginPageFailedLogin(object sender, FailedLoginArgs args)
        {
            SetStatus(null);
            NavigateToCredentialsPage(_ctx, "Login unsuccessful. Careful you don't get locked out.");
        }

        private DeltekPageBase[] _pages;

        private void TimesheetPageFoundTimesheet(object sender, FoundTimesheetArgs args)
        {
            SetStatus(null); 
            if (args.Timesheet.IsMissingTimeForToday(_ctx))
            {
                SetTrayAlert("Missing timesheet for today!");
            }
            else
            {
                _ctx.SchedulerService.ResetTimer(_ctx, OnTimeToCheckDeltek);
                Databind();
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

        private void CheckNow_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        public void ShowBrowser(bool showBrowser)
        {
            StatusPanel.Visibility = showBrowser ? Visibility.Collapsed : Visibility.Visible;
            Browser.Visibility = showBrowser ? Visibility.Visible : Visibility.Collapsed;
            var browserIsEmpty = Browser.Document == null || ((HTMLDocument) Browser.Document).url == null;
            if (browserIsEmpty)
            {
                Login();
            }
        }

        private void NextCheckTime_Click(object sender, RoutedEventArgs e)
        {
            _statusViewModel.SetNextCheckTimeEditable(true);
        }
    }
}
