using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using DeltekReminder.DesktopApp.Pages;
using DeltekReminder.Lib;
using DeltekReminder.Lib.Utils;
using log4net;
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
        private static readonly ILog _log = MyLogManager.GetLogger(typeof(Status));

        public Status()
        {
            InitializeComponent();
            Browser.LoadCompleted += Browser_LoadCompleted;

            _ctx = DeltekReminderUiContext.GetInstance();
            
            _statusViewModel = new StatusViewModel(_ctx);
            _statusViewModel.CheckTimeChanged += StatusViewModelOnCheckTimeChanged;
            DataContext = _statusViewModel;
        }

        private void BrowserTimedOut()
        {
            SetStatus(null);
            _log.Error("Browser timed out");
        }

        private void StatusViewModelOnCheckTimeChanged(object sender, CheckTimeChangedArgs args)
        {
            _ctx.Settings.SetCheckTime(args.NewCheckTime);
            ResetTimer();
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
            if (!IsPerformingAsyncOperation()) return;
            
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
            if (IsPerformingAsyncOperation())
            {
                _log.Warn("Attempted to log in while already simultaneously checking status.");
                return;
            }
            
            _log.Debug("Attempting to check status");
            
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
            NavigateWithRetry(loginPageUri);
        }

        private void NavigateWithRetry(Uri loginPageUri)
        {
            try
            {
                Browser.Navigate(loginPageUri);
            }
            catch (Exception ex)
            {
                var delay = new Random().Next(10, 100);
                var message = string.Format("Got an error on Navigate, delaying {0} ms and trying again in case this is a deadlock issue.", delay);
                _log.Warn(message, ex);
                Thread.Sleep(delay);
                Browser.Navigate(loginPageUri);
            }
        }

        private void OnGetTimesheetError(object sender, OnErrorArgs args)
        {
            SetStatus(null);
            _log.Error("Error retrieving timesheet", args.Exception);
            var hasEverLoggedInSuccessfully = _ctx.Settings.LastSuccessfulLogin.HasValue;
            if (hasEverLoggedInSuccessfully)
            {
                SendErrorReport sendErrorReport = new SendErrorReport(_ctx, args.Exception);
                sendErrorReport.ShowDialog();
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
            if (!IsPerformingAsyncOperation()) return;

            SetStatus(null);
            NavigateToCredentialsPage(_ctx, "Login unsuccessful. Careful you don't get locked out.");
        }

        private DeltekPageBase[] _pages;

        private void TimesheetPageFoundTimesheet(object sender, FoundTimesheetArgs args)
        {
            if (!IsPerformingAsyncOperation()) return;
            
            SetStatus(null);
            var shouldShowAlert = _ctx.SchedulerService.ShouldShowAlert(_ctx, args.Timesheet);
            if (shouldShowAlert)
            {
                var timesheetStartsAfterToday = args.Timesheet.Days[0].Date > _ctx.Now;
                if (timesheetStartsAfterToday)
                {
                    SetTrayAlert("No active timesheet");
                }
                else
                {
                    var thisIsTheLastDayOfTheTimePeriod = args.Timesheet.Days.Last().Date.SameDay(_ctx.Now);
                    if (thisIsTheLastDayOfTheTimePeriod)
                    {
                        SetTrayAlert("Sign your timesheet");
                    }
                    else
                    {
                        ShowTimePicker(args.Timesheet);
                    }
                }
            }
            else
            {
                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            _ctx.SchedulerService.ResetTimer(_ctx, OnTimeToCheckDeltek);
            Databind();
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs args)
        {
            OnNavigatedToNewPage();
        }

        private void OnNavigatedToNewPage()
        {
            var document = (HTMLDocument)Browser.Document;

            var navigationError = document.url.StartsWith("res://ieframe.dll");
            if (navigationError)
            {
                BrowserTimedOut();
                return;
            }

            var currentPage = _pages.FirstOrDefault(i => i.OnThisPage(_ctx, Browser));
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
            OnNavigatedToNewPage();
        }

        private void CheckNow_Click(object sender, RoutedEventArgs e)
        {
            Login();

            // reset the timer during check now as a way to reset a timer that's gotten out of sync for whatever reason
            ResetTimer();
        }

        public void ShowBrowser(bool showBrowser)
        {
            StatusPanel.Visibility = showBrowser ? Visibility.Collapsed : Visibility.Visible;
            Browser.Visibility = showBrowser ? Visibility.Visible : Visibility.Collapsed;
            var browserIsEmpty = Browser.Document == null || ((HTMLDocument) Browser.Document).url == null;
            if (browserIsEmpty && showBrowser)
            {
                Login();
            }
        }

        private void NextCheckTime_Click(object sender, RoutedEventArgs e)
        {
            _statusViewModel.SetNextCheckTimeEditable(true);
        }

        public async Task<bool> SetHoursForToday(SetHoursForTodayArgs setHoursForTodayArgs)
        {
            var timesheetPage = new TimesheetPage();
            if (timesheetPage.OnThisPage(_ctx, Browser))
            {
                return await timesheetPage.SetHoursForToday(_ctx, Browser, setHoursForTodayArgs.Hours);
            }
            
            // todo: pass in SetHoursForToday as a param for what to do when you find a timesheet
            Login();
            return false;
        }
    }
}
