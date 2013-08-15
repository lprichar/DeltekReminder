using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using DeltekReminder.Lib;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// This works for Deltek v9.0.1.0 build 9.0.1.144
    /// </summary>
    public partial class MainWindow
    {
        private readonly DeltekReminderUiContext _ctx;
        private FrameworkElement _loadingAnimation;
        private TaskbarIcon _taskbarIcon;

        public MainWindow()
        {
            InitializeComponent();
            _ctx = DeltekReminderUiContext.GetInstance();
            NavigateToInitialPage();
            var hasEverSuccessfullyLoggedIn = _ctx.Settings.LastSuccessfulLogin.HasValue;
            if (hasEverSuccessfullyLoggedIn)
            {
                SetWindowVisible(false);
            }
            else
            {
                AddDeltekReminderToStartup();
            }
        }

        private static void AddDeltekReminderToStartup()
        {
            if (!System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) return;

            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            string startPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\Deltek Reminder\Deltek Reminder.appref-ms";
            rkApp.SetValue("Deltek Reminder", startPath);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var versionNumber = GetElementByName<TextBlock>("VersionLabel");
            versionNumber.Text = _ctx.NavigationHelper.GetVersion().ToString();
        }

        private void SetWindowVisible(bool visible)
        {
            Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (visible)
            {
                Activate();
            }
        }

        private void NavigateToInitialPage()
        {
            if (_ctx.Settings.LastSuccessfulLogin.HasValue)
            {
                _ctx.NavigationHelper.ShowStatusPage(NavigationService, showBrowser: false);
            }
            else
            {
                _ctx.NavigationHelper.ShowCredentialsPage(NavigationService);
            }
        }

        public void ShowTimePicker(Timesheet timesheet)
        {
            if (TaskbarIcon.SupportsCustomToolTips)
            {
                var baloon = new TimePickerBaloon(timesheet);
                baloon.OpenTimesheet += (sender, e) => OpenTimesheet(TaskbarIcon);
                baloon.SetHoursForToday += async (sender, args) => await _ctx.NavigationHelper.ShowStatusPage(NavigationService, args); ;
                TaskbarIcon.ShowCustomBalloon(baloon, PopupAnimation.Fade, null);
            }
            else
            {
                TaskbarIcon.ShowBalloonTip("Deltek Reminder", "Your timesheet is missing", BalloonIcon.Error);
            }
        }
        
        public void SetTrayAlert(string message)
        {
            if (TaskbarIcon.SupportsCustomToolTips)
            {
                var baloon = new GenericBaloon {Message = message};
                baloon.OpenTimesheet += (sender, e) => OpenTimesheet(TaskbarIcon);
                TaskbarIcon.ShowCustomBalloon(baloon, PopupAnimation.Fade, null);
            }
            else
            {
                TaskbarIcon.ShowBalloonTip("Deltek Reminder", message, BalloonIcon.Error);
            }
        }

        private void OpenTimesheet(TaskbarIcon taskbarIcon)
        {
            SetWindowVisible(true);
            taskbarIcon.CloseBalloon();
            WindowState = WindowState.Maximized; // not a big fan of this, but Deltek seems to require a huge width and height in order to display well
            _ctx.NavigationHelper.ShowStatusPage(NavigationService, showBrowser: true);
        }

        public void SetStatus(string statusText)
        {
            var showStatus = !string.IsNullOrEmpty(statusText);
            var statusTextElement = GetElementByName<TextBlock>("StatusText");
            SetVisible(LoadingAnimation, showStatus);
            SetVisible(statusTextElement, showStatus);
            if (statusText != null)
                statusTextElement.Text = statusText;
        }

        private TaskbarIcon TaskbarIcon
        {
            get { return _taskbarIcon ?? (_taskbarIcon = GetElementByName<TaskbarIcon>("TaskbarIcon")); }
        }

        private FrameworkElement LoadingAnimation
        {
            get { return _loadingAnimation ?? (_loadingAnimation = GetElementByName<FrameworkElement>("LoadingAnimation")); }
        }

        public bool IsLoadingAnimationVisible()
        {
            return LoadingAnimation.Visibility == Visibility.Visible;
        }

        public void SetVisible(FrameworkElement element, bool visible)
        {
            element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            SetWindowVisible(false);
        }

        private bool _allowDirectNavigation = false;
        private readonly Duration _duration = new Duration(TimeSpan.FromSeconds(.2));
        
        private void MainWindow_OnNavigating(object sender, NavigatingCancelEventArgs navArgs)
        {
            var contentPresenter = GetElementByName<ContentPresenter>("MainContentPresenter");
            if (contentPresenter == null) return;
            
            if (!_allowDirectNavigation)
            {
                navArgs.Cancel = true;

                DoubleAnimation animation0 = new DoubleAnimation {From = 1, To = 0, Duration = _duration};
                animation0.Completed += (s2, e2) => SlideCompleted(navArgs);
                contentPresenter.BeginAnimation(OpacityProperty, animation0);
            }
            _allowDirectNavigation = false;
        }

        private T GetElementByName<T>(string name) where T : FrameworkElement
        {
            if (Template == null) return null;
            return Template.FindName(name, this) as T;
        }

        private void SlideCompleted(NavigatingCancelEventArgs navArgs)
        {
            _allowDirectNavigation = true;
            if (navArgs.NavigationMode != NavigationMode.New) return;
            if (navArgs.Uri != null)
                Navigate(navArgs.Uri);
            else
                Navigate(navArgs.Content);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                (ThreadStart)delegate
                {
                    var contentPresenter = GetElementByName<ContentPresenter>("MainContentPresenter");
                    DoubleAnimation animation0 = new DoubleAnimation {From = 0, To = 1, Duration = _duration};
                    contentPresenter.BeginAnimation(OpacityProperty, animation0);
                });
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TaskbarIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            SetWindowVisible(true);
        }

        private void Credentials_OnClick(object sender, RoutedEventArgs e)
        {
            _ctx.NavigationHelper.ShowCredentialsPage(NavigationService);
        }

        private void Status_OnClick(object sender, RoutedEventArgs e)
        {
            _ctx.NavigationHelper.ShowStatusPage(NavigationService, showBrowser: false);
        }

        private void Browser_OnClick(object sender, RoutedEventArgs e)
        {
            _ctx.NavigationHelper.ShowStatusPage(NavigationService, showBrowser: true);
        }
    }
}
