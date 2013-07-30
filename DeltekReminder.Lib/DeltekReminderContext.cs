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

        public virtual DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DeltekReminderSettings Settings
        {
            get { return _settings; }
        }
    }
}
