using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NSubstitute;
using NUnit.Framework;

namespace WinFormData.Tests
{
    [TestFixture]
    public class FluentTest
    {
        private  IContainer container;
        private  IFileWatcher fw;
        private  IXmlHelper api;
        private  IMainModel model;
        private  ILifetimeScope scope;
        
        protected IXmlHelper Api;
        protected IXmlFeed Feed;

        [SetUp]
        public void SetUp()
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
            model = scope.Resolve<IMainModel>();
            
        }

        [Test]
        public void FluentTest1()
        {
           
        }


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
            //model2 = scope.Resolve<IMainModel>();

            builder.RegisterType<XmlHelper>().As<IXmlHelper>();
            builder.RegisterType<FSymbol>().As<IFSymbol>();

            var f = scope.Resolve<IFSymbol>();

            var s1 = "9501.JP";
            var s2 = "7203.JP";
            var s3 = "7201.JP";

            new Thread(() =>
            f.SetSymbol(s1)
             .SetFActionType(FActionType.In)
             .Type(FType.Stock)
             .Share(500)
             .BidAtMarket()
             .WaitSeconds(5)
             .MarketInLong()
             .WriteConsole(s1)).Start();

            var f2 = scope.Resolve<IFSymbol>();

            new Thread(() =>
            f2.SetSymbol(s2)
             .SetFActionType(FActionType.In)
             .Type(FType.Stock)
             .Share(800)
             .BidAtMarket()
             .WaitSeconds(10)
             .MarketInLong()
             .WriteConsole(s2)).Start();


            var f3 = scope.Resolve<IFSymbol>();

            new Thread(() =>
            f3.SetSymbol(s3)
             .SetFActionType(FActionType.In)
             .Type(FType.Stock)
             .Share(600)
             .BidAtMarket()
             .WaitSeconds(12)
             .MarketInLong()
             .WriteConsole(s3)).Start(); ;


            Console.WriteLine("nonono");
            Console.WriteLine("nonono1");
            Thread.Sleep(15000);

        }

        [Test]
        public void FluentEntryTesting()
        {
            new FSymbol(api)
                .SetSymbol("9501.JP")
                .Share(200)
                .BidAtMarket()
                .WaitSeconds(10)
                .MarketInLong();
            
        }

        [Test]
        public void FluentExitHalfTesting()
        {
            new FSymbol(api)
                .SetSymbol("9501.JP")
                .SetExitSharePercentage(50)
                .ExitBidOrOffer()
                .WaitSeconds(10)
                .ExitMarket();

        }


        // Use this for exiting full amount bid first and exit 
        [Test]
        public void FluentSoftFlat()
        {
            new FSymbol(api)
                .SetSymbol("9501.JP")
                .SetExitSharePercentage(100)
                .WaitSeconds(10)
                .FlatPosition();
        }

        [Test]
        public void FluentFlatTesting()
        {
            new FSymbol(api)
                .SetSymbol("9501.JP")
                .FlatPosition();
        }
    }
}
