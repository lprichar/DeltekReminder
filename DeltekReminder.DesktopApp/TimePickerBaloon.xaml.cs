﻿using System.Windows;
using System.Windows.Documents;
using DeltekReminder.Lib;

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

        protected virtual void InvokeSetHoursForToday(decimal hours)
        {
            var handler = SetHoursForToday;
            if (handler != null) handler(this, new SetHoursForTodayArgs {  Hours = hours});
        }

        public TimePickerBaloon(Timesheet timesheet)
        {
            InitializeComponent();
            _timesheet = timesheet;
            var projectWithMostHours = _timesheet.GetProjectWithMostHours();
            if (projectWithMostHours != null)
                ProjectName.Text = projectWithMostHours.ChargeDescription;
        }

        private void Hours_OnClick(object sender, RoutedEventArgs e)
        {
            decimal hours = GetHours(sender);
            InvokeSetHoursForToday(hours);
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
    }

    public delegate void SetHoursForToday(object sender, SetHoursForTodayArgs args);

    public class SetHoursForTodayArgs
    {
        public decimal Hours { get; set; }
    }
}
