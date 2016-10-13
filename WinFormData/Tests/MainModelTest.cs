using System;
using System.Reflection;
using System.Threading;
using Autofac;
using NUnit.Framework;
using NSubstitute;

namespace WinFormData.Tests
{
    /// <summary>
    /// The testing will only work for 9501.JP
    /// </summary>
    [TestFixture]
    public class MainModelTest
    {
        private MainModel model;
        private XmlHelper xmlHelper;
        private FakeXmlFeed fakeXmlFeed;

        protected IXmlHelper Api;
        protected IXmlFeed Feed;

        [SetUp]
        public void SetUp()
        {
            fakeXmlFeed = new FakeXmlFeed();
            Api = Substitute.For<IXmlHelper>();
            Feed = Substitute.For<IXmlFeed>();
            xmlHelper = new XmlHelper(fakeXmlFeed);
            model = new MainModel(xmlHelper);

            Global.ShareSize = 200;
        }

        [Test]
        public void Lv1GainPositionTest()
        {
            Global.UpdateSymbol("9501.JP", 440, 435, "Buy", 2000);
            var gridData = model.GetLv1GainPosition();

            Assert.AreEqual(gridData.Count, 1);
        }

        [Test]
        public void SymbolDictionaryTest()
        {
            Global.UpdateSymbol("ABC.TO", 2.7, 2.31, "Buy", 2000);

            var y = Global.GetSymbol("ABC.TO");
            Assert.AreEqual(y.Side, "Buy");

            var z = Global.GetStopLossForSymbol("ABC.TO");
            Assert.AreEqual(z, 2.31);

            var a = Global.Tradelist2;
            Assert.AreEqual(1, a.Count);

            Global.RemoveSymbol("ABC.TO");
            var b = Global.Tradelist2;
            Assert.AreEqual(0, b.Count);
        }

        [Test]
        public void MassLv1Test()
        {
            //var x = model.ExecuteGetMassL1("9522.JP");
            //Assert.NotNull(x);

            var y = model.ExecuteGetMassL1("9501.JP");
            Assert.NotNull(y);
        }

        [Test]
        public void GetTableTest()
        {
            Global.UpdateSymbol("ABC.TO", 2.9, 2.31, "Buy", 2000);
            Global.UpdateSymbol("XYZ.TO", 5.1, 4.33, "Sell", 400);

            var x = model.GetLv1GainPosition();
            Assert.AreEqual(x.Count, 2);

        }

        [Test]
        public void XmlApiTest()
        {
            var x = xmlHelper.GetL1("ABC.TO");
            var xtype = x.GetType();
            Assert.AreEqual(xtype.Name, "Lv1Quote");


            var y = xmlHelper.GetOpenPositionForSymbol("9501.JP");
            var ytype = y.GetType();
            Assert.AreEqual(ytype.Name, "OpenPosition");

            var z = xmlHelper.GetOpenPositionForSymbol("ABC.TO");
            string zMess = "";
            if (z == null)
            {
                zMess = "no entry";
            }
            Assert.AreEqual(zMess, "no entry");

        }

        [Test]
        public void RemainShareTest()
        {
            var x = model.RemainingShare(25, 200);
            Assert.AreEqual(x, 200);

            var y = model.RemainingShare(0, 200);
            Assert.AreEqual(y, 200);

            var z = model.RemainingShare(300, 200);
            Assert.AreEqual(z, 0);

            var a = model.RemainingShare(6300, 200);
            Assert.AreEqual(a, 0);
        }

        [Test]
        public void GetL1()
        {
            var x = xmlHelper.GetL1("9501.JP");
            Assert.NotNull(x);
        }

        [Test]
        public void RegisterStockTest()
        {
            xmlHelper.RegisterL1("9501.JP");
            Assert.AreEqual(1,1);
        }

        [Test]
        public void GetAllPositionsTest()
        {
            var all = xmlHelper.GetAllOpenPosition();
            Assert.AreEqual(2,all.Count);
        }

        [Test]
        public void ExitHalfTest()
        {
            model.ExecuteBuyBidInExitHalfOffer("9501.JP",200,5555.2355);
        }

        [Test]
        public void GetOrderStateTest()
        {
            var order = xmlHelper.GetOrderState("1232312");
            Assert.AreEqual(order.State, State.Cancelled);
        }

        [Test]
        public void ExecuteBuyOnStop()
        {
            model.ExecuteBuyStop("9501.JP", 447);
            
        }

        [Test]
        public void NameWith2Dots()
        {
            model.ExecuteBuyBidInMarketAfterTime("RDS.A.NY", 200, 0);
        }

        [Test]
        public void BuyBidAndMarketAfterTimeTest()
        {
            //model.ExecuteBuyBidInMarketAfterTime("9501.JP", 200, 0);
            //model.ExecuteSellOfferInMarketAfterTime("9501.JP", 400, 10);
            //model.ExecuteBidInAndCancelAfterTime("9501.JP", 300, 0);
            model.ExecuteSoftSpecial("7201.JP",0);
        }

        public void TestRemainingShares()
        {
            var fakeXmlFeed1 = new FakeXmlFeed();
            var xmlHelper1 = new XmlHelper(fakeXmlFeed1);
            var model1 = new MainModel(xmlHelper1);

            var remain = model1.RemainingShare(1, 5);
            Assert.AreEqual(4, remain);

            var remain2 = model1.RemainingShare(-150, 500);
            Assert.AreEqual(350,remain2);
        }

        /// <summary>
        ///  This is a FSymbol test
        /// </summary>
        private  IContainer container;
        private  IFileWatcher fw;
        private  IXmlHelper api;
        private  IMainModel model2;
        private  ILifetimeScope scope;


        [Test]
        public void FSymbolTest()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .AsImplementedInterfaces();

            builder.RegisterType<FakeXmlFeed>().As<IXmlFeed>();
            Global.IsFakePoco = true;

            container = builder.Build();
            scope = container.BeginLifetimeScope();

            fw = scope.Resolve<IFileWatcher>();
            api = scope.Resolve<IXmlHelper>();
            model2 = scope.Resolve<IMainModel>();

            builder.RegisterType<XmlHelper>().As<IXmlHelper>();
            builder.RegisterType<FSymbol>().As<IFSymbol>();

            var f = scope.Resolve<IFSymbol>();


            f.SetSymbol("9501.JP")
             .SetFActionType(FActionType.In)
             .Type(FType.Stock)
             .Share(500)
             .BidAtMarket()
             .WaitSeconds(5)
             .MarketInLong()
             .WriteConsole("5");

            //var f2 = scope.Resolve<IFSymbol>();

            //new Thread(() => 
            //f2.SetSymbol("9501.JP")
            // .SetFActionType(FActionType.In)
            // .Type(FType.Stock)
            // .Share(800)
            // .BidAtMarket()
            // .WaitSeconds(10)
            // .MarketInLong()
            // .WriteConsole("10")).Start();


            //var f3 = scope.Resolve<IFSymbol>();

            //new Thread(() => 
            //f3.SetSymbol("9501.JP")
            // .SetFActionType(FActionType.In)
            // .Type(FType.Stock)
            // .Share(600)
            // .BidAtMarket()
            // .WaitSeconds(12)
            // .MarketInLong()
            // .WriteConsole("12")).Start(); ;


            Console.WriteLine("nonono");
            Console.WriteLine("nonono1");
            Thread.Sleep(1000);
        }

        public void FSymbolTest2()
        {
            var f1 = scope.Resolve<IFSymbol>();
            f1.BidAtPrice(123.00); //This should throw exception when trying to access Symbol

            var f2 = scope.Resolve<IFSymbol>();
            f2.SetSymbol("A")
              .SetSymbol("B"); //This should throw exception 

            var f3 = scope.Resolve<IFSymbol>();
            f3.SetSymbol("A")
              .SetFActionType(FActionType.Exit); //Example to set the FActionType to Exit

        }

        [Test]
        public void IntegerDivisionTest()
        {
            int i = 1;
            var x = i/2;

            Assert.Greater(i,0);
        }
        
    }

    public enum Tstatus
    {
        Good = 1,
        Bad = 2,
    }

   
    


}
