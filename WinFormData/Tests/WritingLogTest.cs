using NUnit.Framework;

namespace WinFormData.Tests
{
    [TestFixture]
    public class LogTest
    {
        [Test]
        public void LogTest1()
        {
            var log = new Log();
            log.Updatelog("this is testing!");
        }
    }
}    

