using System;
using System.Collections.Generic;
using System.XmlHelper;

namespace WinFormData
{
    public class Lv1Quote
    {
        public string Symbol { get; set; }
        public double BidPrice { get; set; }
        public double BidSize { get; set; }
        public double AskPrice { get; set; }
        public double AskSize { get; set; }
        public DateTime MarketTime { get; set; }
    }

    public class OpenPosition
    {
        public string Symbol { get; set; }
        public string Market { get; set; }
        public int Volume { get; set; }
        public string Side { get; set; }
    }

    //<OrderState state="3" description="Accepted" fillCount="0"/>
    //<OrderState state="8" description="Cancelled" fillCount="0"/>
    //<OrderState state="7" description="Filled" fillCount="1" />
    public enum State
    {
        Filled,
        Accepted,
        Cancelled
    }

    public class Order
    {
        public string OrderId { get; set; }
        public State State { get; set; }
    }

    public class Symbol
    {
        public string Name { get; set; }
        public double StopLoss { get; set; }
        public double InPrice { get; set; }
        public string Side { get; set; }
        public int Volume { get; set; }

    }

    public class DisplayGrid
    {
        public string Symbol { get; set; }
        public double BidPrice { get; set; }
        public double AskPrice { get; set; }
        public double InPrice { get; set; }
        public double PositionSize { get; set; }
        public string Side { get; set; }
        public double StopLoss { get; set; }
        public double Gain { get; set; }

    }

    public class SymbolMarket
    {
        public string Symbol { get; set; }
        public string Market { get; set; }
    }

    internal static class Global
    {
        //this is for buy/sell stop
        public static int StopEntryTime { get; set; }
        public static int StopCheckInterval { get; set; }

        public static string TraderId { get; set; }
        public static string PproPath { get; set; }
        public static string EsignalPath { get; set; }
        public static int ShareSize { get; set; }
        public static int NumOfRetries { get; set; }
        public static bool IsFakePoco { get; set; }


        private static Dictionary<string, Symbol> tlist2;

        public static Dictionary<string, Symbol> Tradelist2
        {
            get { return tlist2 ?? (tlist2 = new Dictionary<string, Symbol>()); }
            set { tlist2 = value; }
        }

        public static Symbol GetSymbol(string symbol)
        {
            Symbol value;
            var success = Tradelist2.TryGetValue(symbol, out value);
            if (!success)
            {
                var s = new Symbol {Name = symbol};
                Tradelist2.Add(symbol, s);
                return s;
            }
            return value;
        }

        public static void RemoveSymbol(string symbol)
        {
            Tradelist2.Remove(symbol);
        }

        public static void UpdateSymbol(string symbol, double inrice, double stopLoss, string side, int volume)
        {
            var s = new Symbol {Name = symbol, InPrice = inrice, Side = side, StopLoss = stopLoss, Volume = volume};
            Tradelist2[symbol] = s;

        }

        public static double GetStopLossForSymbol(string symbol)
        {
            var s = GetSymbol(symbol);
            return s.StopLoss;
        }

        private static List<string> AmList
        {
            get { 
                var sh = new SettingHelper();
                return sh.GetAmList();
            }
        }

        public static List<string> AmExceptionList { get; set; }

        public static List<string> NqExceptionList { get; set; }

        //private static readonly List<string> AmExceptionList =
        //    new List<string>
        //        {
        //            "SPY",
        //            "XLE",
        //            "XLF"
        //        };

        //private static readonly List<string> NqExceptionList = 
        //    new List<string>
        //        {
        //            "QQQ"
        //        };

        public static List<string> GetNqExceptionList()
        {
            return NqExceptionList;
        } 

        public static List<string> GetAmExceptionList()
        {
            return AmExceptionList;
        }

        public static void AddToExceptionList(string sm)
        {
            AmExceptionList.Add(sm);
        }
    }

    public class Chrome : Site
    {
        public string ChromeId { get; set; }
    }

}
