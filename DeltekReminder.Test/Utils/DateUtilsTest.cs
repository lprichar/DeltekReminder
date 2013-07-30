using System;
using DeltekReminder.Lib.Utils;
using NUnit.Framework;

namespace DeltekReminder.Test.Utils
{
    [TestFixture]
    public class DateUtilsTest
    {
        [Test]
        public void DoIt()
        {
            var actual = DateUtils.ParsePeriodEndingDate("Jul 31, 2013");
            Assert.AreEqual(new DateTime(2013, 07, 31), actual);
        }
    }
}
