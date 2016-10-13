using System;
using System.Linq;
using System.Threading;
using System.XmlHelper;
using System.Xml.Linq;


namespace WinFormData
{
    public interface IXmlFeed
    {
        XDocument CancelSellOrdersForSymbol(string symbol);
        XDocument CancelBuyOrdersForSymbol(string symbol);
        XDocument FlattenSymbol(string symbol);
        XDocument GetOpenPositionForSymbol(string symbol);
        XDocument GetAllOpenPosition();
        XDocument ExecuteOrder(string side, string symbol, double price, int shares);
        XDocument GetOrderState(string orderId);
        XDocument RegisterL1(string symbol);
        XDocument GetLv1(string symbol);
        XDocument GetOrderNumber(string execId);
        bool RegisterAll();
    }
   
    public class XmlFeed : IXmlFeed
    {
        public XDocument CancelSellOrdersForSymbol(string symbol)
        {
            var url = String.Format("http://localhost:8682/CancelOrder?type=all&symbol={0}&side=offer", symbol);
            return url.GetData();
        }

        public XDocument CancelBuyOrdersForSymbol(string symbol)
        {
            var url = String.Format("http://localhost:8682/CancelOrder?type=all&symbol={0}&side=bid", symbol);
            return url.GetData();
        }

        public XDocument FlattenSymbol(string symbol)
        {
            var url = String.Format("http://localhost:8682/Flatten?symbol={0}", symbol);
            return url.GetData();
        }

        public XDocument GetOpenPositionForSymbol(string symbol)
        {
            var url = String.Format("http://localhost:8682/GetOpenPositions?user={0}&symbol={1}", Global.TraderId, symbol);
            return url.GetData();
        }

        public XDocument GetAllOpenPosition()
        {
            var url = String.Format("http://localhost:8682/GetOpenPositions?user={0}", Global.TraderId);
            return url.GetData();
        }

        public XDocument ExecuteOrder(string side, string symbol, double price, int shares)
        {
            var url = GetBuySellUrl(side, symbol, price, shares);
            return url.GetData();
        }

        public XDocument GetOrderState(string orderId)
        {
            var url = String.Format("http://localhost:8682/GetOrderState?ordernumber={0}", orderId);
            return url.GetData();
        }

        public XDocument GetOrderNumber(string execId)
        {
            var url = String.Format("http://localhost:8682/GetOrderNumber?requestid={0}", execId);
            return url.GetData();
        }

        public XDocument RegisterL1(string symbol)
        {
            var url = String.Format("http://localhost:8682/Register?symbol={0}&feedtype=L1&output=bytype", symbol);
            return url.GetData();
        }

        public XDocument GetLv1(string symbol)
        {
            var url = String.Format("http://localhost:8682/GetLv1?symbol={0}", symbol);
            return url.GetData();
        }

        public bool RegisterAll()
        {
            var regList = new[]
                {
                    "http://localhost:8682/Register?region=3&feedtype=OSTAT&output=bytype",
                    "http://localhost:8682/Register?region=3&feedtype=PAPIORDER&output=bytype",
                    "http://localhost:8682/Register?region=1&feedtype=OSTAT&output=bytype",
                    "http://localhost:8682/Register?region=1&feedtype=PAPIORDER&output=bytype"
                };

            try
            {
                foreach (var item in regList)
                {
                    item.GetData();
                    Thread.Sleep(200);
                }
            }catch
            {
                return false;
            }
            return true;
        }

        private string GetBuySellUrl(string side, string symbol, double price, int shares)
        {
            bool isBuy;

            if (side == "Buy")
                isBuy = true;
            else if (side == "Sell")
                isBuy = false;
            else
                throw new NotSupportedException(string.Format("Unsupported side {0}", side));

            var arr = symbol.Split('.').ToArray();
            var country = arr[arr.Length - 1]; 
            //symbol.Split('.').ToArray()[1];

            if (country == "CM")
            {
                var orderName = "CME Buy CME Limit DAY";

                if (!isBuy)
                    orderName = "CME Sell CME Limit DAY";

                return String.Format(
                    "http://localhost:8682/ExecuteOrder?symbol={0}&limitprice={1}&shares={2}&ordername={3}",
                    symbol, price, shares, orderName);
            }

            if (country == "HF")
            {
                var orderName = "HKFE Buy HKFE Limit DAY";

                if (!isBuy)
                    orderName = "HKFE Sell HKFE Limit DAY";

                return String.Format(
                    "http://localhost:8682/ExecuteOrder?symbol={0}&limitprice={1}&shares={2}&ordername={3}",
                    symbol, price, shares, orderName);
            }

            if (country == "BF")
            {
                var orderName = "BMF Buy BMF Limit DAY";

                if (!isBuy)
                    orderName = "BMF Sell BMF Limit DAY";

                return String.Format(
                    "http://localhost:8682/ExecuteOrder?symbol={0}&limitprice={1}&shares={2}&ordername={3}",
                    symbol, price, shares, orderName);

            }

            if (country == "JF")
            {
                var orderName = "OSE Buy OSE Limit DAY";

                if (!isBuy)
                    orderName = "OSE Sell OSE Limit DAY";

                return String.Format(
                    "http://localhost:8682/ExecuteOrder?symbol={0}&limitprice={1}&shares={2}&ordername={3}",
                    symbol, price, shares, orderName);
                
            }

            if (country =="AM" || country =="NY") //This is NYSE stocks, suffix is AM, NY 
            {
                var orderName = "ARCA Buy ARCX Limit DAY";

                if (!isBuy)
                    orderName = "ARCA Sell->Short ARCX Limit DAY";

                return String.Format(
                    "http://localhost:8682/ExecuteOrder?symbol={0}&limitprice={1}&shares={2}&ordername={3}",
                    symbol, price, shares, orderName);
            }

            if (country == "NQ")
            {
                var orderName = "NSDQ Buy NSDQ Limit DAY";
                
                if (!isBuy)
                    orderName = "NSDQ Sell->Short NSDQ Limit DAY";

                return String.Format(
                    "http://localhost:8682/ExecuteOrder?symbol={0}&limitprice={1}&shares={2}&ordername={3}",
                    symbol, price, shares, orderName);
            }

            if (country == "JP")
            {
                var orderName = "CHIJ Buy CHI-XJapan Limit DAY";

                if (!isBuy)
                    orderName = "CHIJ Sell->Short CHI-XJapan Limit DAY";

                return String.Format(
                    "http://localhost:8682/ExecuteOrder?symbol={0}&limitprice={1}&shares={2}&ordername={3}",
                    symbol, price, shares, orderName);
            }

            if (country == "TO")
            {
                var orderName = "TSX Buy SweepSOR Limit ANON DAY";

                if (!isBuy)
                    orderName = "TSX Sell->Short SweepSOR Limit ANON DAY";

                return String.Format(
                    "http://localhost:8682/ExecuteOrder?symbol={0}&limitprice={1}&shares={2}&ordername={3}",
                    symbol, price, shares, orderName);
            }

            
            //throw new NotSupportedException("The symbol or country is not supported");

            // Assume that it's New York stock
            var orderName1 = "ARCA Buy ARCX Limit DAY";

            if (!isBuy)
                orderName1 = "ARCA Sell->Short ARCX Limit DAY";

            return String.Format(
                "http://localhost:8682/ExecuteOrder?symbol={0}&limitprice={1}&shares={2}&ordername={3}",
                symbol, price, shares, orderName1);
        }
    }
}