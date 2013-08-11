using System;
using NUnit.Framework;

namespace DeltekReminder.Test
{
    [TestFixture]
    public class DeltekReminderSettingsTest : TestBase
    {
        [Test]
        public void GetCheckTimeForToday()
        {
            var ctx = GetCtx();
            ctx.MockNow = new DateTime(2010, 1, 1, 1, 1, 1);
            ctx.Settings.CheckTime = "5:15 PM";
            var actual = ctx.Settings.GetCheckTimeForToday(ctx);
            Assert.AreEqual(new DateTime(2010, 1, 1, 17, 15, 0, 0), actual);
        }
    }
}
