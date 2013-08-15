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
        public void GetTodayDay()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2010, 1, 2, 5, 5, 5);
            var timesheet = new Timesheet();
            var firstDay = new TimesheetDay {Date = new DateTime(2010, 1, 1)};
            var secondDay = new TimesheetDay { Date = new DateTime(2010, 1, 2) };
            var thirdDay = new TimesheetDay { Date = new DateTime(2010, 1, 3) };
            timesheet.Days.Add(firstDay);
            timesheet.Days.Add(secondDay);
            timesheet.Days.Add(thirdDay);
            var actual = timesheet.GetTodayDay(ctx);
            Assert.AreEqual(secondDay, actual);
        }

        [Test]
        public void GetProjectWithMostHours_ZeroProject_ReturnNull()
        {
            var timesheet = new Timesheet();
            var project = timesheet.GetProjectWithMostHours();
            Assert.IsNull(project);
        }

        [Test]
        public void GetProjectWithMostHours_OneProject_ReturnIt()
        {
            var timesheet = new Timesheet();
            var theProject = new TimesheetProject();
            timesheet.Projects.Add(theProject);
            var actualProject = timesheet.GetProjectWithMostHours();
            Assert.AreEqual(actualProject, theProject);
        }

        [Test]
        public void GetProjectWithMostHours_OneProjectHasMoreHours_ReturnIt()
        {
            var timesheet = new Timesheet();
            const int smallProjectRow = 0;
            const int bigProjectRow = 1;
            var smallProject = new TimesheetProject
                {
                    Row = smallProjectRow
                };
            var bigProject = new TimesheetProject
                {
                    Row = bigProjectRow
                };
            timesheet.Projects.Add(smallProject);
            timesheet.Projects.Add(bigProject);
            timesheet.Entries.Add(new TimesheetEntry
                {
                    Row = smallProjectRow,
                    Hours = 4,
                    Column = 0
                });
            timesheet.Entries.Add(new TimesheetEntry
                {
                    Row = bigProjectRow,
                    Hours = 8,
                    Column = 1,
                });
            timesheet.Entries.Add(new TimesheetEntry
                {
                    Row = bigProjectRow,
                    Hours = 8,
                    Column = 2,
                });
            var actualProject = timesheet.GetProjectWithMostHours();
            Assert.AreEqual(bigProject, actualProject);
        }

        [Test]
        public void IsMissingTimeForToday_NoTime_Missing()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2013, 1, 2);
            var timesheet = new Timesheet();
            Assert.IsTrue(timesheet.IsMissingTimeForToday(ctx));
        }

        public Timesheet GetTimesheetWithEntry(DateTime date, decimal hours)
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
