//#define pretendIts4_59_pm

using System;

namespace DeltekReminder.Lib
{
    public class DeltekReminderContext
    {

#if (pretendIts4_59_pm)
        private readonly DateTime _fakeStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 2, 16, 59, 58);  // Today at 4:59 PM
        private readonly DateTime _startedApp = DateTime.Now;
#endif

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
#if (pretendIts4_59_pm)
            get { return _fakeStartDate + (DateTime.Now - _startedApp); }
#else
            get { return DateTime.Now; }
#endif
        }

        public DeltekReminderSettings Settings
        {
            get { return _settings; }
        }
    }
}
