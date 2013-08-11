using System.Windows.Threading;
using DeltekReminder.Lib;
using log4net;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog _log = MyLogManager.GetLogger(typeof(App));

        public App()
        {
            Dispatcher.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _log.Error("Unhandled global error", e.Exception);
            var ctx = DeltekReminderUiContext.GetInstance();
            SendErrorReport sendErrorReport = new SendErrorReport(ctx, e.Exception);
            sendErrorReport.ShowDialog();
            e.Handled = true;
        }
    }
}
