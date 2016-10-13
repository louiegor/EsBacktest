using System;
using System.Threading;

namespace WinFormData
{
    public interface IFSymbol : IFSymbolExecute
    {
        //Basic
        IFSymbol SetSymbol(string s);
        IFSymbol Share(int shareSize);
        IFSymbol SetFActionType(FActionType t);
        IFSymbol Type(FType type);
        IFSymbol SetExitSharePercentage(int percent);
    }

    public interface IFSymbolExecute
    {
        IFSymbolExecute BidAtPrice(double price);
        IFSymbolExecute BidAtMarket();
        IFSymbolExecute OfferAtPrice(double price);
        IFSymbolExecute OfferAtMarket();
        IFSymbolExecute MarketInLong();
        IFSymbolExecute MarketInShort();

        IFSymbolExecute WaitSeconds(int seconds);
        IFSymbolExecute WaitMinutes(int minutes);

        IFSymbolExecute ExitMarket();
        IFSymbolExecute ExitBidOrOffer();

        IFSymbolExecute CancelBuyOrders();
        IFSymbolExecute CancelSellOrders();
        IFSymbolExecute CancelAllOrders();

        IFSymbolExecute FlatPosition();

        //for testing
        IFSymbolExecute WriteConsole(string text);

    }

    //This is testing for fluent method
    //To use it, create a FSymbol class
    public class FSymbol: IFSymbol
    {
        private readonly IXmlHelper api;
        private string symbol;

        private string Symbol
        {
            get
            {
                if (symbol == null)
                    throw new Exception("Did you forgot to init before using this? Please call Init(string symbol).");

                return symbol;
            }
            set
            {
                if (symbol != null)
                    throw new Exception("You should not set symbol more than once. Please recreate your FSymbol");

                symbol = value;
                api.RegisterL1(symbol);
            }
        }

        //Either a Stock or Future
        private FType FType { get; set; }
        private ErrorResult Result { get; set; }

        // The amount of share you wanna buy or sell in the transaction
        private int ShareSize { get; set; }
        
        
        // The following are for market exit
        // Whether you are entering or exiting the market
        private FActionType FActionType { get; set; }

        // This is the amount of share in your open position
        private int InitialCurrentPostionShare
        {
            get
            {
                var o = InitialOpenPos;

                if (o == null)
                    return 0;

                return o.Volume;
            }
        }

        // This is the type of postion type - Long or Short 
        // For exiting the postion if Current Position is Long
        // then you should Sell or Offer 
        private FPositionType CurrentPositionType
        {
            get
            {
                var o = InitialOpenPos;

                if (o == null || o.Side == "B")
                    return FPositionType.Long;

                return FPositionType.Short;
            }
        }

        private OpenPosition initopenPos;
        private OpenPosition InitialOpenPos
        {
            get
            {
                if (initopenPos != null)
                    return initopenPos;

                if (FActionType == FActionType.Exit)
                {
                    initopenPos = api.GetOpenPositionForSymbol(Symbol);

                    if (initopenPos == null)
                        Error("Cannot get Open Position");

                    return initopenPos;
                }

                return new OpenPosition
                    {
                        Market = "Other",
                        Side ="B",
                        Symbol=Symbol,
                        Volume = 0
                    };
            }
            
        }

        // Initial Condition
        public FSymbol(IXmlHelper api)
        {
            this.api = api;

            //Defaults
            FActionType = FActionType.In;
            FType = FType.Stock;
            Result = new ErrorResult();
        }

        // Basic: Set Action (Entry or Exit) 
        public IFSymbol SetFActionType(FActionType t)
        {
            FActionType = t;
            if (t == FActionType.Exit)
            {
                initopenPos = api.GetOpenPositionForSymbol(Symbol);
            }

            return this;
        }

        // Basic: Set Type (Stock or Future) 
        public IFSymbol Type(FType type)
        {
            FType = type;
            return this;
        }

        // Update Share Size for Entry
        public IFSymbol Share(int shareSize)
        {
            ShareSize = shareSize;
            return this;
        }
        
        public IFSymbol SetSymbol(string s)
        {
            Symbol = s;
            return this;
        }



        // Update Share Size for Exit
        public IFSymbol SetExitSharePercentage(int percent)
        {
            FLog.AddFormLogMessage(String.Format("Set the share size to {0} percent for {1}", percent, Symbol));

            var curOpenPos = api.GetOpenPositionForSymbol(Symbol);
            if (curOpenPos.Volume == 0) return this;

            var newShare = initopenPos.Volume * percent / 100;

            ShareSize = Math.Abs(newShare);

            return this;
        }

        public IFSymbolExecute WriteConsole(string text)
        {
            Console.Write(text);
            return this;
        }

        public IFSymbolExecute BidAtPrice(double price)
        {
            var curOpenPos = api.GetOpenPositionForSymbol(Symbol);
            if (curOpenPos.Volume != 0 && curOpenPos.Side != "B")
                api.FlattenSymbol(Symbol);

            var remaining = GetRemainingShareFromOpenPos(curOpenPos);

            api.CancelBuyOrdersForSymbol(Symbol);
            api.ExecuteOrder("Buy", Symbol, price, remaining);
            FLog.AddFormLogMessage(String.Format("Bid In @{0}", price));

            return this;
        }

        public IFSymbolExecute BidAtMarket()
        {
            var l1 = ExecuteGetL1();

            if (l1.Symbol == null)
                Error(String.Format("Cannot get Lv1 for {0}", Symbol));

            if (GetStatus() == Fstatus.Error) return this;

            return BidAtPrice(l1.BidPrice);
        }

        public IFSymbolExecute OfferAtPrice(double price)
        {
            var curOpenPos = api.GetOpenPositionForSymbol(Symbol);
            if (curOpenPos.Volume != 0 && curOpenPos.Side == "B")
                api.FlattenSymbol(Symbol);

            var remaining = GetRemainingShareFromOpenPos(curOpenPos);
            api.CancelSellOrdersForSymbol(Symbol);
            api.ExecuteOrder("Sell", Symbol, price, remaining);
            FLog.AddFormLogMessage(String.Format("Offer In @{0}", price));

            return this;
        }

        public IFSymbolExecute OfferAtMarket()
        {
            var l1 = ExecuteGetL1();

            if (l1.Symbol == null)
                Error(String.Format("Cannot get Lv1 for {0}", Symbol));

            if (GetStatus() == Fstatus.Error) return this;

            return OfferAtPrice(l1.AskPrice);
        }

        public IFSymbolExecute MarketInLong()
        {
            var l1 = ExecuteGetL1();

            if (l1.Symbol == null)
                Error(String.Format("Cannot get Lv1 for {0}", Symbol));

            if (GetStatus() == Fstatus.Error) return this;

            
            var curOpenPos = api.GetOpenPositionForSymbol(Symbol);
            if (curOpenPos.Volume != 0 && curOpenPos.Side != "B")
                api.FlattenSymbol(Symbol);

            var remaining = GetRemainingShareFromOpenPos(curOpenPos);

            if (remaining > 0)
            {
                api.CancelBuyOrdersForSymbol(Symbol);
                var buyPrice = l1.AskPrice;
                api.ExecuteOrder("Buy", Symbol, buyPrice, remaining);
                FLog.AddFormLogMessage(String.Format("Market In @{0}", buyPrice));
            }

            return this;
        }

        public IFSymbolExecute MarketInShort()
        {
            var l1 = ExecuteGetL1();

            if (l1.Symbol == null)
                Error(String.Format("Cannot get Lv1 for {0}", Symbol));

            if (GetStatus() == Fstatus.Error) return this;
            
            var curOpenPos = api.GetOpenPositionForSymbol(Symbol);
            if (curOpenPos.Volume != 0 && curOpenPos.Side == "B")
                api.FlattenSymbol(Symbol);

            var remaining = GetRemainingShareFromOpenPos(curOpenPos);

            if (remaining > 0)
            {
                api.CancelSellOrdersForSymbol(Symbol);
                var sellPrice = l1.BidPrice;
                api.ExecuteOrder("Sell", Symbol, sellPrice, remaining);
                FLog.AddFormLogMessage(String.Format("Market In @{0}", sellPrice));
            }

            return this;
        }

        public IFSymbolExecute WaitSeconds(int seconds)
        {
            Thread.Sleep(seconds * 1000);
            return this;
        }

        public IFSymbolExecute WaitMinutes(int minutes)
        {
            Thread.Sleep(minutes * 60000);
            return this;
        }

        public IFSymbolExecute ExitMarket()
        {
            var l1 = ExecuteGetL1();

            if (l1.Symbol == null)
                Error(String.Format("Cannot get Lv1 for {0}", Symbol));

            if (GetStatus() == Fstatus.Error) return this;

            var curOpenPos = api.GetOpenPositionForSymbol(Symbol);

            var remaining = GetRemainingShareForExit(curOpenPos);
            if (remaining == 0) return this;

            if (CurrentPositionType == FPositionType.Long)
            {
                var sellPrice = l1.BidPrice;
                api.ExecuteOrder("Sell", Symbol, sellPrice, remaining);
                FLog.AddFormLogMessage(String.Format("Market Out @{0} for {1} shares", sellPrice, remaining));
            }
            else if (CurrentPositionType == FPositionType.Short)
            {
                var buyPrice = l1.AskPrice;
                api.ExecuteOrder("Buy", Symbol, buyPrice, remaining);
                FLog.AddFormLogMessage(String.Format("Market Out @{0} for {1}", buyPrice, remaining));
            }

            return this;
        }

        public IFSymbolExecute ExitBidOrOffer()
        {
            var l1 = ExecuteGetL1();

            if (l1.Symbol == null)
                Error(String.Format("Cannot get Lv1 for {0}", Symbol));

            if (GetStatus() == Fstatus.Error) return this;

            var curOpenPos = api.GetOpenPositionForSymbol(Symbol);

            var remaining = GetRemainingShareForExit(curOpenPos);
            if (remaining == 0) return this;

            if (CurrentPositionType == FPositionType.Long)
            {
                var price = l1.AskPrice;
                api.ExecuteOrder("Sell", Symbol, price, remaining);
                FLog.AddFormLogMessage(String.Format("Offer Out @{0} for {1}", price, remaining));
            }
            else if (CurrentPositionType == FPositionType.Short)
            {
                var price = l1.BidPrice;
                api.ExecuteOrder("Buy", Symbol, price, remaining);
                FLog.AddFormLogMessage(String.Format("Bid In @{0} for {1}", price, remaining));
            }
            return this;
        }

        public IFSymbolExecute CancelBuyOrders()
        {
            api.CancelBuyOrdersForSymbol(Symbol);
            return this;
        }

        public IFSymbolExecute CancelSellOrders()
        {
            api.CancelSellOrdersForSymbol(Symbol);
            return this;
        }

        public IFSymbolExecute CancelAllOrders()
        {
            api.CancelBuyOrdersForSymbol(Symbol);
            api.CancelSellOrdersForSymbol(Symbol);
            return this;
        }

        public IFSymbolExecute FlatPosition()
        {
            FLog.AddFormLogMessage(String.Format("Executing Flat for {0},", Symbol));
            api.FlattenSymbol(Symbol);
            return this;
        }

        public Fstatus GetStatus()
        {
            if (ShareSize == 0)
            {
                Result.Error("share size is 0");
            }
            return Result.GetStatus();
        }

        private Lv1Quote ExecuteGetL1()
        {
            var l1 = new Lv1Quote();
            try
            {
                l1 = api.GetL1(Symbol);
            }
            catch //(Exception e )
            {
                return l1;
            }

            return l1;
        }

        private int GetRemainingShareForExit(OpenPosition curOpenPos)
        {
            return ShareSize - (InitialCurrentPostionShare - Math.Abs(curOpenPos.Volume));
        }

        private int GetRemainingShareFromOpenPos(OpenPosition curOpenPos)
        {
            var inPositionShare = curOpenPos == null ? 0 : Math.Abs(curOpenPos.Volume);
            return RemainingShare(inPositionShare, ShareSize);
        }

        private int RemainingShare(int positionShare, int wantToBuyShare)
        {
            var result = 0;
            if ((positionShare / wantToBuyShare) < 1)
            {
                result = wantToBuyShare - positionShare;
                return result;
            }
            return result;
        }
        
        //private void Filled(string message)
        //{
        //    FLog.AddFormLogMessage(message);
        //    Result.Filled(message);
        //}

        private void Error(string message)
        {
            FLog.AddFormLogMessage(message);
            Result.Error(message);
        }


    }

    public enum Fstatus
    {
        Filled = 0,
        Good = 1,
        Error = 2,
    }

    public enum FActionType
    {
        In = 0,
        Exit = 1
    }

    public enum FPositionType
    {
        Long = 0,
        Short = 1
    }

    public enum FType
    {
        Future,
        Stock
    }

    public class ErrorResult
    {
        public ErrorResult()
        {
            Status =  Fstatus.Good;
        }

        private string Msg { get; set; }
        private Fstatus Status { get; set; }

        public Fstatus GetStatus()
        {
            return Status;
        }

        public void Filled(string message)
        {
            Msg = message;
            Status = Fstatus.Filled;
        }

        public void Error(string message)
        {
            Msg = message;
            Status = Fstatus.Error;
        }
    }
}
