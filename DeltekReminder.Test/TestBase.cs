using System;
using DeltekReminder.Lib;

namespace DeltekReminder.Test
{
    public class TestBase
    {
        public DeltekReminderContextFake GetCtx()
        {
            return new DeltekReminderContextFake();
        }
    }

    public class DeltekReminderContextFake : DeltekReminderContext
    {
        public DeltekReminderContextFake()
        {
            MockNow = new DateTime(2010, 1, 1);
        }

        public DateTime MockNow { get; set; }
        
        public override DateTime Now
        {
            get { return MockNow; }
        }
    }
}
