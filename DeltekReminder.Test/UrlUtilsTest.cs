using DeltekReminder.Lib;
using NUnit.Framework;

namespace DeltekReminder.Test
{
    [TestFixture]
    public class UrlUtilsTest
    {
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
