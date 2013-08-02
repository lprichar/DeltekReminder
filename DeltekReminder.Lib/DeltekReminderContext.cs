using System;

namespace DeltekReminder.Lib
{
    public class DeltekReminderContext
    {
        public DeltekReminderContext()
        {
            _settings = DeltekReminderSettings.GetSettings();
        }

        private readonly DeltekReminderSettings _settings;
        private SchedulerService _schedulerService;

        public SchedulerService SchedulerService
        {
            get { return _schedulerService ?? (_schedulerService = new SchedulerService()); }
        }

        public virtual DateTime Now
        {
            get { return DateTime.Now; }
            //get { return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 59, 55); } // Today at 4:59 PM
        }

        public DeltekReminderSettings Settings
        {
            get { return _settings; }
        }
    }
}
