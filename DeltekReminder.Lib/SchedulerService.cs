using System;
using System.Threading;

namespace DeltekReminder.Lib
{
    public class SchedulerService
    {
        private Timer _timer;
        private DateTime? _nextCheck = null;
        private const int REMIND_USER_IN_MINUTES = 15;

        public void EnsureTimerExists(DeltekReminderContext ctx, TimerCallback onTimeToCheckDeltek)
        {
            if (_timer != null) return;
            
            _timer = new Timer(onTimeToCheckDeltek);
            var destinationTime = GetNextTimeToCheckDeltek(ctx);
            _nextCheck = destinationTime;
            var timeUntilDestinationTime = destinationTime - ctx.Now;
            _timer.Change(timeUntilDestinationTime, new TimeSpan(0, REMIND_USER_IN_MINUTES, 0));
        }

        public void ResetTimer(DeltekReminderContext ctx, TimerCallback onTime)
        {
            if (_timer != null) _timer.Dispose();
            _timer = null;
            _nextCheck = null;
            EnsureTimerExists(ctx, onTime);
        }

        public DateTime GetNextTimeToCheckDeltek(DeltekReminderContext ctx)
        {
            var todayAtCheckTime = new DateTime(ctx.Now.Year, ctx.Now.Month, ctx.Now.Day, 17, 0, 0);
            if (todayAtCheckTime > ctx.Now) return todayAtCheckTime;
            return todayAtCheckTime.AddDays(1);
        }

        public string GetNextTimeToCheckAsText()
        {
            if (_timer == null || _nextCheck == null) return "";
            return _nextCheck.Value.ToLongTimeString();
        }
        
        public string GetNextDayToCheckAsText()
        {
            if (_timer == null || _nextCheck == null) return "";
            return _nextCheck.Value.ToLongDateString();
        }
    }
}
