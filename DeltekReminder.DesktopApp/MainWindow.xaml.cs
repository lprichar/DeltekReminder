using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// This works for Deltek v9.0.1.0 build 9.0.1.144
    /// </summary>
    public partial class MainWindow
    {
        private readonly DeltekReminderUiContext _ctx;

        public MainWindow()
        {
            InitializeComponent();
            _ctx = DeltekReminderUiContext.GetInstance();
            NavigationService.Navigate(GetInitialPage());
            if (_ctx.Settings.LastSuccessfulLogin.HasValue)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private Uri GetInitialPage()
        {
            if (_ctx.Settings.LastSuccessfulLogin.HasValue)
            {
                return _ctx.NavigationHelper.StatusPage;
            }
            return _ctx.NavigationHelper.CredentialsPage;
        }

        public void SetTrayAlert(string message)
        {
            TaskbarIcon taskbarIcon = GetElementByName<TaskbarIcon>("TaskbarIcon");
            taskbarIcon.ShowBalloonTip("Deltek Reminder", message, BalloonIcon.Error);
        }
        
        public void SetStatus(string statusText)
        {
            var showStatus = !string.IsNullOrEmpty(statusText);
            var loadingAnimation = GetElementByName<FrameworkElement>("LoadingAnimation");
            var statusTextElement = GetElementByName<TextBlock>("StatusText");
            SetVisible(loadingAnimation, showStatus);
            SetVisible(statusTextElement, showStatus);
            if (statusText != null)
                statusTextElement.Text = statusText;
        }

        public void SetVisible(FrameworkElement element, bool visible)
        {
            element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowState != WindowState.Minimized)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
            }
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
            WindowState = WindowState.Normal;
        }
    }
}
