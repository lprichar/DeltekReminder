using System;
using NUnit.Framework;

namespace DeltekReminder.Test
{
    [TestFixture]
    public class SchedulerServiceTest : TestBase
    {
        [Test]
        public void GetNextTimeToCheckDeltek_StartOfDay_CheckAtEndOfDay()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2010, 1, 1, 9, 0, 0);
            var actual = ctx.SchedulerService.GetNextTimeToCheckDeltek(ctx);
            Assert.AreEqual(new DateTime(2010, 1, 1, 17, 0, 0), actual);
        }
        
        [Test]
        public void GetNextTimeToCheckDeltek_EndOfDay_CheckAtEndOfDayTomorrow()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2010, 1, 31, 18, 0, 0);
            var actual = ctx.SchedulerService.GetNextTimeToCheckDeltek(ctx);
            Assert.AreEqual(new DateTime(2010, 2, 1, 17, 0, 0), actual);
        }
    }
}
