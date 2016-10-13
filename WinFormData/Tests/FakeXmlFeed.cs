using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;

namespace WinFormData.Tests
{
    public class FakeXmlFeed : IXmlFeed
    {
        private readonly string path = ConfigurationManager.AppSettings["Mock.Dir"];

        public XDocument RegisterL1(string symbol)
        {
            var p = path + "4-1GetLv1Empty.xml";
            //var p = path + "RandomGarbage.xml";
            return XDocument.Load(p);
        }
        public XDocument CancelSellOrdersForSymbol(string symbol)
        {
            var p = path + @"10CancelOrder.xml";
            return XDocument.Load(p);
        }

        public XDocument CancelBuyOrdersForSymbol(string symbol)
        {
            var p = path + @"10CancelOrder.xml";
            return XDocument.Load(p);
        }

        public XDocument FlattenSymbol(string symbol)
        {
            var p = path + @"9Flatten.xml";
            return XDocument.Load(p);
        }

        public XDocument GetAllOpenPosition()
        {
            var p = path + @"11ALLopenposition.xml";
            return XDocument.Load(p);
        }

        public XDocument GetOpenPositionForSymbol(string symbol)
        {
            var p = path + @"8GetOpenPositions.xml";
            return XDocument.Load(p);
        }

        public XDocument GetOrderNumber(string execId)
        {
            var p = path + @"6GetOrderNumber.xml";
            return XDocument.Load(p);
        }

        public XDocument ExecuteOrder(string side, string symbol, double price, int shares)
        {
            var p = path + "5ExecuteOrder.xml";
            return XDocument.Load(p);
        }

        public XDocument GetOrderState(string orderId)
        {
            var p = path + "13OrderAccepted.xml";
            return XDocument.Load(p);
        }

        public XDocument GetLv1(string symbol)
        {
           var p = path + "4GetLv1.xml";
            return XDocument.Load(p);
        }

        public bool RegisterAll()
        {
            return true;
        }
    }

}