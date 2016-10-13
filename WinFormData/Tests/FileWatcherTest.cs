using System;
using System.Threading;
using NUnit.Framework;

namespace WinFormData.Tests
{
    [TestFixture]
    public class FileWatcherTest
    {
        private FileWatcher fw;
        private MainModel model;
        private XmlHelper xh;
        private FakeXmlFeed fakeXmlFeed;

        [SetUp]
        public void Setup()
        {
            fakeXmlFeed = new FakeXmlFeed();
            xh = new XmlHelper(fakeXmlFeed);
            model = new MainModel(xh);
            fw = new FileWatcher(model);
        }

        [Test]
        public void FileReaderTest()
        {
            var x = fw.FileReader(@"C:\CODE\lgTest\PproXml\1Register.xml");
            Assert.NotNull(x);
        }

        [Test]
        public void XmlExecuteOrderTest()
        {
            var x = xh.ExecuteOrder("Buy", "9501.JP", 460, 200);
            Assert.NotNull(x);
        }

        [Test]
        public void FileWatchingTest()
        {
            Global.EsignalPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Interactive Data\FormulaOutput\";
            Global.StopCheckInterval = 3000;
            Global.StopEntryTime = 90000;
            var test = new Thread(() => fw.Watch());
            test.Start();
            Thread.Sleep(180000);
            Assert.NotNull(Global.EsignalPath);
            // new Thread(() => model.ExecuteBuyOrder(name, double.Parse(stopLoss), xh)).Start();
            //fw.Watch();
        }
    }
}
