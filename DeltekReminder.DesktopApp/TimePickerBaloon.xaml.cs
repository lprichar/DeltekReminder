using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using DeltekReminder.Lib;
using Hardcodet.Wpf.TaskbarNotification;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// Interaction logic for TimePickerBaloon.xaml
    /// </summary>
    public partial class TimePickerBaloon
    {
        private readonly Timesheet _timesheet;
        public event SetHoursForToday SetHoursForToday;
        public event OpenTimesheet OpenTimesheet;

        protected virtual void InvokeOpenTimesheet()
        {
            var handler = OpenTimesheet;
            if (handler != null) handler(this, new OpenTimesheetArgs());
        }

        protected async virtual Task<bool> InvokeSetHoursForToday(decimal hours)
        {
            var handler = SetHoursForToday;
            if (handler != null)
            {
                var args = new SetHoursForTodayArgs {Hours = hours};
                return await handler(this, args);
            }
            return false;
        }

        public TimePickerBaloon(Timesheet timesheet)
        {
            InitializeComponent();
            _timesheet = timesheet;
            var projectWithMostHours = _timesheet.GetProjectWithMostHours();
            if (projectWithMostHours != null)
                ProjectName.Text = projectWithMostHours.ChargeDescription;
        }

        private async void Hours_OnClick(object sender, RoutedEventArgs e)
        {
            decimal hours = GetHours(sender);
            LoadingAnimation.Visibility = Visibility.Visible;
            var success = await InvokeSetHoursForToday(hours);
            LoadingAnimation.Visibility = Visibility.Collapsed;
            if (success)
            {
                Close();
            }
            else
            {
                ErrorSaving.Visibility = Visibility.Visible;
            }
        }

        private static decimal GetHours(object sender)
        {
            var hyperlink = (Hyperlink) sender;
            var inline = (Run) hyperlink.Inlines.FirstInline;
            var text = inline.Text;
            return decimal.Parse(text);
        }

        private void OpenTimesheet_Click(object sender, RoutedEventArgs e)
        {
            InvokeOpenTimesheet();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Close()
        {
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }

    public delegate Task<bool> SetHoursForToday(object sender, SetHoursForTodayArgs args);

    public class SetHoursForTodayArgs
    {
        public decimal Hours { get; set; }
    }
}
