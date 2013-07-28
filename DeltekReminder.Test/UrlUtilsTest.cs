using System;
using DeltekReminder.Lib;
using NUnit.Framework;

namespace DeltekReminder.Test
{
    [TestFixture]
    public class UrlUtilsTest
    {
        [Test]
        public void GetLoginPage()
        {
            var actual = UrlUtils.GetLoginPage("https://te01.neosystems.net");
            Assert.AreEqual(new Uri("https://te01.neosystems.net/DeltekTC/welcome.msv"), actual);
        }
        
        [Test]
        public void OnDesktopPage_YesWithParams()
        {
            Assert.IsTrue(UrlUtils.OnDesktopPage(new Uri("https://te01.neosystems.net/DeltekTC/TimeCollection.msv?p=1")));
        }

        [Test]
        public void OnDesktopPage_YesWithoutParams()
        {
            Assert.IsTrue(UrlUtils.OnDesktopPage(new Uri("https://te01.neosystems.net/DeltekTC/TimeCollection.msv")));
        }

        [Test]
        public void OnDesktopPage_LoginPage_No()
        {
            Assert.IsFalse(UrlUtils.OnDesktopPage(new Uri("https://te01.neosystems.net/DeltekTC/welcome.msv")));
        }

        [Test]
        public void GetBase_WithHttps_HttpsRemains()
        {
            var actual = UrlUtils.GetBase("https://te01.neosystems.net/DeltekTC/welcome.msv");
            Assert.AreEqual("https://te01.neosystems.net", actual);
        }
        
        [Test]
        public void GetBase_WithHttp_HttpRemains()
        {
            var actual = UrlUtils.GetBase("http://te01.neosystems.net/DeltekTC/welcome.msv");
            Assert.AreEqual("http://te01.neosystems.net", actual);
        }
        
        [Test]
        public void GetBase_BaseAndQuerystring_OnlyBase()
        {
            var actual = UrlUtils.GetBase("https://te01.neosystems.net?p=1");
            Assert.AreEqual("https://te01.neosystems.net", actual);
        }
        
        [Test]
        public void GetBase_Null_Null()
        {
            var actual = UrlUtils.GetBase(null);
            Assert.IsNull(actual);
        }
    }
}
