using NUnit.Framework;

namespace WinFormData.Tests
{
    [TestFixture]
    public class XmlFeedTest
    {
        private FileWatcher fw;
        private MainModel model;
        private XmlHelper xh;
        private XmlFeed xmlFeed;

        [SetUp]
        public void Setup()
        {
            xmlFeed = new XmlFeed();
            xh = new XmlHelper(xmlFeed);
            model = new MainModel(xh);
            fw = new FileWatcher(model);
        }
        [Test]
        public void RealXmlFeedTest()
        {
            xmlFeed.ExecuteOrder("Buy", "RDS.A.NY", 24, 200);
        }

        [Test]
        public void RealXmlFeed1Test()
        {
            xmlFeed.ExecuteOrder("Buy", "C.NY", 24, 200);
        }
    }
}
