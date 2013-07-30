using System;
using System.Collections.Generic;
using DeltekReminder.Lib;
using NUnit.Framework;

namespace DeltekReminder.Test
{
    [TestFixture]
    public class TimesheetTest : TestBase
    {
        [Test]
        public void IsMissingTimeForToday_NoTime_Missing()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2013, 1, 2);
            var timesheet = new Timesheet();
            Assert.IsTrue(timesheet.IsMissingTimeForToday(ctx));
        }
        
        public Timesheet GetTimesheetWithEntry(DateTime date, double hours)
        {
            return new Timesheet
            {
                Entries = new List<TimesheetEntry>
                        {
                            new TimesheetEntry
                                {
                                    Date = date,
                                    Hours = hours
                                },
                        }
            };
        }

        [Test]
        public void IsMissingTimeForToday_TimeTodayMidnight_NotMissing()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2013, 1, 2, 0, 0, 0, 0);
            var timesheet = GetTimesheetWithEntry(new DateTime(2013, 1, 2), 8);
            Assert.IsFalse(timesheet.IsMissingTimeForToday(ctx));
        }
        
        [Test]
        public void IsMissingTimeForToday_TimeYesterday_IsMissing()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2013, 1, 2, 23, 59, 59, 59);
            var timesheet = GetTimesheetWithEntry(new DateTime(2013, 1, 1), 8);
            Assert.IsTrue(timesheet.IsMissingTimeForToday(ctx));
        }
        
        [Test]
        public void IsMissingTimeForToday_TodayAlmostOverButHasTime_IsNotMissing()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2013, 1, 1, 23, 59, 59, 59);
            var timesheet = GetTimesheetWithEntry(new DateTime(2013, 1, 1), 8);
            Assert.IsFalse(timesheet.IsMissingTimeForToday(ctx));
        }
        
        [Test]
        public void IsMissingTimeForToday_ZeroHours_IsMissing()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2013, 1, 2);
            var timesheet = GetTimesheetWithEntry(new DateTime(2013, 1, 2), 0);
            Assert.IsTrue(timesheet.IsMissingTimeForToday(ctx));
        }

        [Test]
        public void IsMissingTimeForToday_WeekendWithZeroHours_IsNotMissing()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2013, 1, 5, 0, 0, 0, 0);
            var timesheet = new Timesheet();
            Assert.IsFalse(timesheet.IsMissingTimeForToday(ctx));
        }
    }
}
