using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WinFormData
{
    public interface IXmlHelper
    {
        bool RegisterAll();
        void CancelSellOrdersForSymbol(string symbol);
        void CancelBuyOrdersForSymbol(string symbol);
        void FlattenSymbol(string symbol);
        bool RegisterL1(string symbol);
        OpenPosition GetOpenPositionForSymbol(string symbol);
        Order GetOrderState(string orderId);
        string ExecuteOrder(string side, string symbol, double price, int shares);
        string GetOrderNumber(string executionNumber);
        Lv1Quote GetL1(string symbol);
        List<OpenPosition> GetAllOpenPosition();
    }

    public static class FormHelper
    {
        public static void FormSetText(string text)
        {
            if (Form1.Form1Edit!= null)
                Form1.Form1Edit.SetText(text);
        }
    }

    public static class Extension
    {
        public static int RoundUpToNearest100(this int x)
        {
            var result = (int) Math.Round(x/100.0);
            if (x > 0 && result == 0)
            {
                result = 1;
            }

            int final = 100*result;
            return final;
        }
    }

   public class XmlHelper : IXmlHelper
    {
        private readonly IXmlFeed feed;
        private readonly Log log;

        public XmlHelper(IXmlFeed feed)
        {
            this.feed = feed;
            log = new Log();
        }

        public bool RegisterAll()
        {
            var success = feed.RegisterAll();

            if (success)
            {
                FormHelper.FormSetText("Registered Sucessfully");
                log.Updatelog("Registered Sucessfully");
                return true;
            }
            else
            {
                FormHelper.FormSetText("Register Failed");
                log.Updatelog("Register Failed");
                return false;
            }
        }

        public void CancelSellOrdersForSymbol(string symbol)
        {
            var xdoc = feed.CancelSellOrdersForSymbol(symbol);
            log.Updatelog("Execute Cancel Sell Order");
            try
            {
                xdoc.XPathSelectElements("//Content");
            }
            catch (Exception e)
            {
                log.Updatelog(e.Message);
                FormHelper.FormSetText(e.Message);
            }
        }

        public void CancelBuyOrdersForSymbol(string symbol)
        {
            log.Updatelog("Execute Cancel Buy Order");
            var xdoc = feed.CancelBuyOrdersForSymbol(symbol);
            try
            {
                xdoc.XPathSelectElements("//Content");
            }
            catch (Exception e)
            {
                log.Updatelog(e.Message);
                FormHelper.FormSetText(e.Message);

            }
        }

       public void FlattenSymbol(string symbol)
       {
           log.Updatelog(String.Format("Flattening symbol {0}",symbol));
           FormHelper.FormSetText("FlattenSymbol: " + symbol);
           var xdoc = feed.FlattenSymbol(symbol);
           try
           {
               xdoc.XPathSelectElements("//Content");
           }
           catch (Exception e)
           {
               log.Updatelog(e.Message);
               FormHelper.FormSetText(e.Message);
           }
       }

       public Order GetOrderState(string orderId)
        {
            var xdoc = feed.GetOrderState(orderId);
            var temp = xdoc.Descendants("OrderState")
                .Select(x =>
                        new
                            {
                                State = x.Attribute("description").Value,
                                OrderId = orderId
                            })
                .ToList()
                .FirstOrDefault();
            
            if (temp==null) return new Order();
            
            var state = new State();
            
            if (temp.State == "Filled") state = State.Filled;
            else if (temp.State == "Accepted") state = State.Accepted;
            else if (temp.State == "Cancelled") state = State.Cancelled;

            return new Order {OrderId = orderId, State = state};
        }

       public List<OpenPosition> GetAllOpenPosition()
        {
            //log.Updatelog("Execute get all position");
            var xdoc = feed.GetAllOpenPosition();
            var oPositions = new List<OpenPosition>();
            foreach (var item in xdoc.Descendants("Position"))
            {
                if (item !=null)
                {
                    var newop = new OpenPosition
                                    {
                                            Symbol = item.Attribute("Symbol").Value,
                                            Market = "Others",//item.Attribute("Market").Value,
                                            Volume = Int32.Parse(item.Attribute("Volume").Value),
                                            Side = item.Attribute("Side").Value,
                                    };
                    oPositions.Add(newop);
                }
            }
           //.Select(x => new OpenPosition
                     //{
                     //    Symbol = x.Attribute("Symbol")==null? .Value,
                     //    Market = x.Attribute("Market").Value,
                     //    Volume = Int32.Parse(x.Attribute("Volume").Value),
                     //    Side = x.Attribute("Side").Value,

                     //})
                     //.ToList();
            return oPositions;
       }

        public OpenPosition GetOpenPositionForSymbol(string symbol)
        {
            log.Updatelog(string.Format("Execute get open position for {0}",symbol));
            var xdoc = feed.GetOpenPositionForSymbol(symbol);
            var oPositions = xdoc.Descendants("Position")
                                 .Select(x => new OpenPosition
                                     {
                                         Symbol = x.Attribute("Symbol").Value,
                                         Market = "Others",//x.Attribute("Market").Value,
                                         Volume = Int32.Parse(x.Attribute("Volume").Value),
                                         Side = x.Attribute("Side").Value,

                                     })
                                 .ToList();

            var o = oPositions.FirstOrDefault(x => x.Symbol == symbol);

            if (o == null)
            {
                var emptyPostion =
                    new OpenPosition
                        {
                            Symbol = symbol, Volume = 0, Side = "None", Market = "None"
                        };
                return emptyPostion;
            }
            
            return o;
        }

        public string ExecuteOrder(string side, string symbol, double price, int shares)
        {
            log.Updatelog(string.Format("Execute {0} for {1} at {2} - {3} shares",side,symbol,price,shares));
            var xdoc = feed.ExecuteOrder(side, symbol, price, shares);
            var execId = xdoc.XPathSelectElements("//Content").Single().Value;
            if (execId == "")
            {
                log.Updatelog(String.Format("Cannot execute {0} for {1}", side, symbol));
                throw new ApplicationException(String.Format("Cannot execute {0} for {1}",side,symbol));
            }
            return execId;
        }

        public string GetOrderNumber(string executionNumber)
        {
            Thread.Sleep(500);
            var xdoc2 = feed.GetOrderNumber(executionNumber);
            var orderNum = xdoc2.XPathSelectElements("//Content").Single().Value;
            if (orderNum == "")
            {
                log.Updatelog(String.Format("The xml feed does not return the Order Number"));
                throw new ApplicationException(String.Format("The xml feed does not return the Order Number"));
            }
            return orderNum;
        }

        public bool RegisterL1(string symbol)
        {
            log.Updatelog(string.Format("Registering symbol {0}", symbol));
            var xdoc = feed.RegisterL1(symbol);
            if (xdoc == null)
            {
                log.Updatelog(String.Format("Failed to register {0}", symbol));
                throw new ApplicationException(String.Format("Failed to register {0}", symbol));
            }

            var x = xdoc.XPathSelectElements("//Content").Single().Value;
            if (x != "")
            {
                log.Updatelog("No response for registering Lv1");
                throw new ApplicationException("No response for registering Lv1");
            }

            return true;

        }
        public Lv1Quote GetL1(string symbol)
        {
            log.Updatelog(string.Format("Get Lv1 for {0}", symbol));
            var xdoc = feed.GetLv1(symbol);

            var lv1S = xdoc.Descendants("Level1Data")
                .Select(x =>
                        new Lv1Quote
                            {
                                Symbol = x.Attribute("Symbol").Value,
                                BidPrice = double.Parse(x.Attribute("BidPrice").Value),
                                AskPrice = double.Parse(x.Attribute("AskPrice").Value),
                                BidSize = double.Parse(x.Attribute("BidSize").Value),
                                AskSize = double.Parse(x.Attribute("AskSize").Value),
                                MarketTime = DateTime.Now
                            }).ToList();

            var current = lv1S.FirstOrDefault(x => x.Symbol == symbol);

            if (current == null)
            {
                log.Updatelog("Cannot get level 1 quote");
                throw new Exception("Cannot get level 1 quote");
            }

            return current;
        }

    }
}




