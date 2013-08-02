using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

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
        }

        private Uri GetInitialPage()
        {
            if (_ctx.Settings.LastSuccessfulLogin.HasValue)
            {
                return _ctx.NavigationHelper.StatusPage;
            }
            return _ctx.NavigationHelper.CredentialsPage;
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
            var contentPresenter = GetContentPresenter();
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

        private ContentPresenter GetContentPresenter()
        {
            if (Template == null) return null;
            return (ContentPresenter)Template.FindName("MainContentPresenter", this);
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
                    var contentPresenter = GetContentPresenter();
                    DoubleAnimation animation0 = new DoubleAnimation {From = 0, To = 1, Duration = _duration};
                    contentPresenter.BeginAnimation(OpacityProperty, animation0);
                });
        }
    }
}
