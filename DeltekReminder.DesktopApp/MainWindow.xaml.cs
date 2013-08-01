using System;
using System.Windows;
using DeltekReminder.Lib;

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
            if (_ctx.Settings.LastSuccessfulDeltekCheck.HasValue)
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
    }
}
