using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace WinFormData
{
    public interface IMainModel
    {
        int RemainingShare(int positionShare, int wantToBuyShare);
        void ExecuteExitHalf(string symbol);
        void ExecuteSoftFlat(string symbol, int? timeToFlat=null);
        void ExecuteSoftSpecial(string symbol, int timeToFlat);
        void ExecuteFlatOrder(string symbol);
        void ExecuteBuyStop(string symbol,double price);
        void ExecuteSellStop(string symbol, double price);
        void ExecuteBuyBidInExitHalfOffer(string symbol, int shareSize, double halfTarget);
        void ExecuteSellOfferInExitHalfBid(string symbol, int shareSize, double halfTarget);
        void ExecuteExitHalfAndMarketAfterTime(string symbol, int min);
        void ExecuteBuyBidInMarketAfterTime(string symbol, int shareSize, int secondToMarket);
        void ExecuteSellOfferInMarketAfterTime(string symbol, int shareSize, int secondToMarket);
        void ExecuteBuyAtPrice(string symbol, double price, int share);
        void ExecuteSellAtPrice(string symbol, double price, int share);
        void ExecuteBidInAndCancelAfterTime(string symbol, int shareSize, int secondToCancel);
        void ExecuteOfferInAndCancelAfterTime(string symbol, int shareSize, int secondToCancel);

        Lv1Quote ExecuteGetL1(string symbol);
        DataTable ConvertToDataTable<T>(IList<T> data);
        List<Lv1Quote> GetLv1GainPosition();
        List<DisplayGrid> GetLv1GainPosition2();

        void WriteConsole(string x);
    }

    public class MainModel : IMainModel
    {
        private readonly IXmlHelper api;
        private Log log;

        public MainModel(IXmlHelper api)
        {
            this.api = api;
            log = new Log();
        }

        public void WriteConsole(string x)
        {
            Console.WriteLine(x);
        }

        public int RemainingShare(int positionShare, int wantToBuyShare)
        {
            var result = 0;
            if ((positionShare/wantToBuyShare) < 1)
            {
                result = wantToBuyShare - positionShare;
                return result;
            }
            return result;
        }

        public void ExecuteSellStop(string symbol, double inPrice)
        {
            Global.StopEntryTime = 90000;
            Global.StopCheckInterval = 3000;
            api.RegisterL1(symbol);
            long elapsedMs = 0;
            var watch = Stopwatch.StartNew();

            FormHelper.FormSetText(String.Format("Execute SellStop {0} @ {1}", symbol, inPrice));

            while (elapsedMs < Global.StopEntryTime)
            {
                var l1 = ExecuteGetL1(symbol);
                if (l1.AskPrice < inPrice)
                {
                    var sellPrice = l1.BidPrice - 0.1;
                    // only try once
                    api.ExecuteOrder("Sell", symbol, sellPrice, Global.ShareSize);
                    log.AddFormLogMessage(String.Format("Price reached. Market In"));
                    break;
                }

                elapsedMs = watch.ElapsedMilliseconds;
                Thread.Sleep(Global.StopCheckInterval);
            }
            watch.Stop();

            var position = api.GetOpenPositionForSymbol(symbol);
            const int stopLoss = 0;

            if (position.Volume != 0)
            {
                Global.UpdateSymbol(symbol, inPrice, stopLoss, position.Side, position.Volume);
                log.AddFormLogMessage(String.Format("Filled {0} for {1}", position.Symbol, position.Volume));
            }
            else
            {
                FormHelper.FormSetText(String.Format("Did not get fill"));
                log.Updatelog(String.Format("Did not get fill"));
            }

            api.CancelBuyOrdersForSymbol(symbol);
        }
        
        public void ExecuteBuyBidInMarketAfterTime(string symbol, int shareSize, int secondToMarket)
        {
            api.RegisterL1(symbol);
            if (secondToMarket == 0)
            {
                ExecuteMarketInLong(symbol, shareSize);
                return;
            }

            //Try to bid
            ExecuteBidInAndCancelAfterTime(symbol, shareSize, secondToMarket);
            
            //If evrything gets filled return
            var remaining = GetRemainingShareForSymbol(EnterType.Buy, symbol, shareSize);
            if (remaining == 0) return;

            //Market In Buy
            ExecuteMarketInLong(symbol,shareSize);
        }


        public void ExecuteBidInAndCancelAfterTime(string symbol, int shareSize, int secondToCancel)
        {
            api.RegisterL1(symbol);
            var l1 = ExecuteGetMassL1(symbol);

            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var remaining = GetRemainingShareForSymbol(EnterType.Buy, symbol, shareSize);

            if (remaining > 0)
            {
                api.CancelBuyOrdersForSymbol(symbol);
                var buyPrice = l1.BidPrice;

                // Tokyo Stock Exchange
                var array = symbol.Split('.');
                if (array[1] != null && array[1].ToLowerInvariant() == "jp")
                    buyPrice = Math.Floor(buyPrice) ;

                // only try once
                api.ExecuteOrder("Buy", symbol, buyPrice, remaining);
                log.AddFormLogMessage(String.Format("Bid In @{0} for {1}", buyPrice, symbol));
            }

            Thread.Sleep(1000 * secondToCancel);
            api.CancelBuyOrdersForSymbol(symbol);
        }

        public void ExecuteMarketInLong(string symbol, int shareSize)
        {
            //Get Level 1
            var l1 = ExecuteGetMassL1(symbol);

            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var remaining = GetRemainingShareForSymbol(EnterType.Buy, symbol, shareSize);

            if (remaining > 0)
            {
                var buyPrice = l1.AskPrice;

                // Tokyo Stock Exchange
                var array = symbol.Split('.');
                if (array[1] != null && array[1].ToLowerInvariant() == "jp")
                    buyPrice = Math.Round(buyPrice, 0) + 2;

                // only try once
                api.ExecuteOrder("Buy", symbol, buyPrice, remaining);
                log.AddFormLogMessage(String.Format("Market In @{0} for {1}", buyPrice, symbol));
            }
        }

        public void ExecuteSellOfferInMarketAfterTime(string symbol, int shareSize, int secondToMarket)
        {
            api.RegisterL1(symbol);
            if (secondToMarket == 0)
            {
                ExecuteMarketInShort(symbol, shareSize);
                return;
            }

            //Try to offer
            ExecuteOfferInAndCancelAfterTime(symbol, shareSize, secondToMarket);

            //If evrything gets filled return
            var remaining = GetRemainingShareForSymbol(EnterType.Sell, symbol, shareSize);
            if (remaining == 0) return;

            //Market in short
            ExecuteMarketInShort(symbol, shareSize);
        }

        public void ExecuteOfferInAndCancelAfterTime(string symbol, int shareSize, int secondToCancel)
        {
            api.RegisterL1(symbol);
            var l1 = ExecuteGetMassL1(symbol);

            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var remaining = GetRemainingShareForSymbol(EnterType.Sell, symbol, shareSize);

            if (remaining > 0)
            {
                api.CancelSellOrdersForSymbol(symbol);
                var sellPrice = l1.AskPrice;

                // Tokyo Stock Exchange
                var array = symbol.Split('.');
                if (array[1] != null && array[1].ToLowerInvariant() == "jp")
                    sellPrice = Math.Ceiling(sellPrice);

                // only try once
                api.ExecuteOrder("Sell", symbol, sellPrice, remaining);
                log.AddFormLogMessage(String.Format("Offer In @{0} for {1}", sellPrice, symbol));
            }

            Thread.Sleep(1000 * secondToCancel);
            api.CancelSellOrdersForSymbol(symbol);
        }

        public void ExecuteMarketInShort(string symbol, int shareSize)
        {
            //Market In Sell
            var l1 = ExecuteGetMassL1(symbol);

            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var remaining = GetRemainingShareForSymbol(EnterType.Sell, symbol, shareSize);

            if (remaining > 0)
            {
                var sellPrice = l1.BidPrice;

                // Tokyo Stock Exchange
                var array = symbol.Split('.');
                if (array[1] != null && array[1].ToLowerInvariant() == "jp")
                    sellPrice = Math.Round(sellPrice, 0) - 1;

                // only try once
                api.ExecuteOrder("Sell", symbol, sellPrice, remaining);
                log.AddFormLogMessage(String.Format("Market In @{0} for {1}", sellPrice, symbol));
            }
        }

        public void ExecuteBuyAtPrice(string symbol, double price, int share)
        {
            api.RegisterL1(symbol);

            //Get Level 1
            var l1 = ExecuteGetL1(symbol);

            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var remaining = GetRemainingShareForSymbol(EnterType.Buy, symbol, share);

            var buyPrice = Math.Round(price,2);

            // Tokyo Stock Exchange
            var array = symbol.Split('.');
            if (array[1] != null && array[1].ToLowerInvariant() == "jp")
                buyPrice = Math.Round(buyPrice, 0) + 2;

            // only try once
            api.ExecuteOrder("Buy", symbol, buyPrice, remaining);
            log.AddFormLogMessage(String.Format("Buy @{0} for {1}", buyPrice, symbol));

        }

        public void ExecuteSellAtPrice(string symbol, double price, int share)
        {
            api.RegisterL1(symbol);

            //Get Level 1
            var l1 = ExecuteGetL1(symbol);

            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var remaining = GetRemainingShareForSymbol(EnterType.Sell, symbol, share);

            var sellPrice = Math.Round(price,2);

            // Tokyo Stock Exchange
            var array = symbol.Split('.');
            if (array[1] != null && array[1].ToLowerInvariant() == "jp")
                sellPrice = Math.Round(sellPrice, 0);

            // only try once
            api.ExecuteOrder("Sell", symbol, sellPrice, remaining);
            log.AddFormLogMessage(String.Format("Sell @{0} for {1}", sellPrice, symbol));
        }

       

        public void ExecuteBuyBidInExitHalfOffer(string symbol, int shareSize, double halfTarget)
        {
            api.RegisterL1(symbol);
            var l1 = ExecuteGetL1(symbol);

            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var openPos = api.GetOpenPositionForSymbol(symbol);
            if (openPos != null && openPos.Side != "B")
            {
                api.FlattenSymbol(symbol);
            }
            var remaining = GetRemainingShareFromOpenPos(openPos, shareSize);

            if (remaining > 0)
            {
                api.CancelBuyOrdersForSymbol(symbol);
                var buyPrice = l1.BidPrice;
                // only try once
                api.ExecuteOrder("Buy", symbol, buyPrice, shareSize);
                FormHelper.FormSetText(String.Format("Bid In @{0}", buyPrice));
                log.Updatelog(String.Format("Bid In @{0}", buyPrice));
            }
            const int oneMinute = 60000;
            Thread.Sleep(1 * oneMinute);
            api.CancelBuyOrdersForSymbol(symbol);

            //check if the thing gets filled
            openPos = api.GetOpenPositionForSymbol(symbol);
            if (openPos !=null && openPos.Side == "B")
            {
                var halfShare = openPos.Volume/2;
                halfShare = Math.Abs(halfShare);
                halfTarget = Math.Round(halfTarget, 2);
                if (symbol.ToLowerInvariant().Contains("mhi"))
                {
                    halfTarget = Math.Round(halfTarget, 0);
                }

                if (halfShare > 0)
                {
                    api.ExecuteOrder("Sell", symbol, halfTarget, halfShare);
                    log.AddFormLogMessage(String.Format("Offer half @{0}", halfTarget));
                }
            }

        }

        public void ExecuteSellOfferInExitHalfBid(string symbol, int shareSize, double halfTarget)
        {
            api.RegisterL1(symbol);
            var l1 = ExecuteGetL1(symbol);

            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var openPos = api.GetOpenPositionForSymbol(symbol);
            if (openPos != null && openPos.Side == "B")
            {
                api.FlattenSymbol(symbol);
            }
            var remaining = GetRemainingShareFromOpenPos(openPos, shareSize);

            if (remaining > 0)
            {
                api.CancelSellOrdersForSymbol(symbol);

                var sellPrice = l1.AskPrice;
                // only try once
                api.ExecuteOrder("Sell", symbol, sellPrice, shareSize);
                log.AddFormLogMessage(String.Format("Offer In @{0}", sellPrice));
            }

            const int oneMinute = 60000;
            Thread.Sleep(10 * oneMinute);
            api.CancelSellOrdersForSymbol(symbol);

            //check if the thing gets filled
            openPos = api.GetOpenPositionForSymbol(symbol);
            if (openPos != null && openPos.Side != "B")
            {
                var halfShare = openPos.Volume / 2;
                halfShare = Math.Abs(halfShare);
                halfTarget = Math.Round(halfTarget, 2);
                if (symbol.ToLowerInvariant().Contains("mhi"))
                {
                    halfTarget = Math.Round(halfTarget, 0);
                }
                if (halfShare > 0)
                {
                    api.ExecuteOrder("Buy", symbol, halfTarget, halfShare);
                    log.AddFormLogMessage(String.Format("Bid half @{0}", halfTarget));
                }
            }
        }

        

        public void ExecuteBuyStop(string symbol, double inPrice)
        {
            Global.StopEntryTime = 90000;
            Global.StopCheckInterval = 3000;
            api.RegisterL1(symbol);
            long elapsedMs = 0;
            var watch = Stopwatch.StartNew();

            FormHelper.FormSetText(String.Format("Execute BuyStop {0} @ {1}", symbol, inPrice));

            while (elapsedMs < Global.StopEntryTime)
            {
                var l1 = ExecuteGetL1(symbol);
                if (l1.BidPrice > inPrice)
                {
                    var buyPrice = l1.AskPrice + 0.1;
                    // only try once
                    api.ExecuteOrder("Buy", symbol, buyPrice, Global.ShareSize);
                    log.AddFormLogMessage(String.Format("Price reached. Market In"));
                    break;
                }

                elapsedMs = watch.ElapsedMilliseconds;
                Thread.Sleep(Global.StopCheckInterval);
            }
            watch.Stop();

            var position = api.GetOpenPositionForSymbol(symbol);
            const int stopLoss = 0;

            if (position.Volume != 0)
            {
                Global.UpdateSymbol(symbol, inPrice, stopLoss, position.Side, position.Volume);
                log.AddFormLogMessage(String.Format("Filled {0} for {1}", position.Symbol, position.Volume));
            }
            else
            {
                log.AddFormLogMessage(String.Format("Did not get fill"));
            }

            api.CancelBuyOrdersForSymbol(symbol);
        }

        public void ExecuteExitHalf(string symbol)
        {
            FormHelper.FormSetText(String.Format("Execute exit half for {0}", symbol));
            
            api.RegisterL1(symbol);
            var l1 = ExecuteGetL1(symbol);
            
            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var openPos = api.GetOpenPositionForSymbol(symbol);
            if (openPos.Volume == 0)
            {
                return;
            }

            var halfShare = openPos.Volume / 2;
            halfShare = Math.Abs(halfShare);

            //check if the thing gets filled
            openPos = api.GetOpenPositionForSymbol(symbol);
            if (openPos != null && openPos.Side == "B")
            {
                api.CancelBuyOrdersForSymbol(symbol);
                if (halfShare > 0)
                {
                    api.ExecuteOrder("Sell", symbol, l1.AskPrice, halfShare);
                    log.AddFormLogMessage(String.Format("Offer half @{0}", l1.AskPrice));
                }
            }
            else
            {
                
                api.CancelSellOrdersForSymbol(symbol);
                if (halfShare > 0)
                {
                    api.ExecuteOrder("Buy", symbol, l1.BidPrice, halfShare);
                    log.AddFormLogMessage(String.Format("Bid half @{0}", l1.BidPrice));
                }
            }

            const int oneMinute = 60000;
            Thread.Sleep(10 * oneMinute);

            api.CancelBuyOrdersForSymbol(symbol);
            api.CancelSellOrdersForSymbol(symbol);
        }
        

        public void ExecuteExitHalfAndMarketAfterTime(string symbol, int second)
        {
            FormHelper.FormSetText(String.Format("Execute exit half for {0}", symbol));

            api.RegisterL1(symbol);
            var l1 = ExecuteGetL1(symbol);

            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            var openPos = api.GetOpenPositionForSymbol(symbol);
            if (openPos.Volume == 0)
                return;

            var initial = Math.Abs(openPos.Volume);

            var halfShare = openPos.Volume / 2;
            halfShare = Math.Abs(halfShare);

            //check if the thing gets filled
            openPos = api.GetOpenPositionForSymbol(symbol);
            if (openPos != null && openPos.Side == "B")
            {
                api.CancelBuyOrdersForSymbol(symbol);
                if (halfShare > 0)
                {
                    api.ExecuteOrder("Sell", symbol, l1.AskPrice, halfShare);
                    log.AddFormLogMessage(String.Format("Offer half @{0}", l1.AskPrice));
                }
            }
            else if (openPos != null && openPos.Side != "B")
            {
                api.CancelSellOrdersForSymbol(symbol);
                if (halfShare > 0)
                {
                    api.ExecuteOrder("Buy", symbol, l1.BidPrice, halfShare);
                    log.AddFormLogMessage(String.Format("Bid half @{0}", l1.BidPrice));
                }
            }
            
            Thread.Sleep(second * 1000);
            api.CancelBuyOrdersForSymbol(symbol);
            api.CancelSellOrdersForSymbol(symbol);

            var openPos1 = api.GetOpenPositionForSymbol(symbol);
            var remaining2 = GetRemainingShareForExit(openPos1, halfShare, initial);

            l1 = ExecuteGetL1(symbol);
            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                return;
            }

            if (openPos1 != null && openPos1.Volume != halfShare && remaining2 > 0)
            {
                if (openPos1.Side == "B")
                {
                    api.ExecuteOrder("Sell", symbol, l1.BidPrice, remaining2);
                    log.AddFormLogMessage(String.Format("Market half @{0}", l1.AskPrice));
                }
                else
                {
                    api.ExecuteOrder("Buy", symbol, l1.AskPrice, remaining2);
                    log.AddFormLogMessage(String.Format("Market half @{0}", l1.BidPrice));
                }
            }
        }
        
        public void ExecuteSoftFlat(string symbol, int? timeToFlat =null)
        {
            var secondsToFlat = 5;
            if (timeToFlat != null) secondsToFlat = (int) timeToFlat;
            FormHelper.FormSetText(String.Format("Execute soft exit for {0}", symbol));
            var openPos = api.GetOpenPositionForSymbol(symbol);
            if (openPos.Volume != 0 && secondsToFlat == 0)
            {
                ExecuteFlatOrder(symbol);
            }
            else if (openPos.Volume != 0 && openPos.Side == "B")
            {
                var vol = Math.Abs(openPos.Volume);
                SoftSell(symbol, vol, secondsToFlat);
            }
            else if (openPos.Volume != 0 && openPos.Side == "S")
            {
                var vol = Math.Abs(openPos.Volume);
                SoftBuy(symbol, vol, secondsToFlat);
            }
            else
            {
                ExecuteFlatOrder(symbol);
            }
        }

        public void ExecuteSoftSpecial(string symbol, int timeToFlat)
        {
            ExecuteSoftFlat(symbol,timeToFlat);
            api.CancelBuyOrdersForSymbol(symbol);
            api.CancelSellOrdersForSymbol(symbol);
            log.AddFormLogMessage(String.Format("Cancel all orders for {0}", symbol));
        }


        private void SoftBuy(string symbol, int remaining, int secondsToMarket)
        {
            var l1 = ExecuteGetL1(symbol);
            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                ExecuteFlatOrder(symbol);
                return;
            }
            var buyPrice = l1.BidPrice;

            log.AddFormLogMessage(String.Format("Buy {0} @ {1} for {2} shares", symbol, buyPrice, remaining));

            api.ExecuteOrder("Buy", symbol, buyPrice, remaining);

            Thread.Sleep(secondsToMarket * 1000);

            var openPos = api.GetOpenPositionForSymbol(symbol);
            api.CancelBuyOrdersForSymbol(symbol);
            
            
            if (openPos.Volume != 0)
            {
                ExecuteFlatOrder(symbol);
            }
            else
            {
                Global.RemoveSymbol(symbol);
            }
        }

        private void SoftSell(string symbol, int remaining, int secondsToMarket)
        {
            var l1 = ExecuteGetL1(symbol);
            if (l1.Symbol == null)
            {
                log.AddFormLogMessage(String.Format("Cannot get Lv1 for {0}", symbol));
                ExecuteFlatOrder(symbol);
                return;
            }
            var sellPrice = l1.AskPrice;

            log.AddFormLogMessage(String.Format("Sell {0} @ {1} for {2} shares", symbol, sellPrice, remaining));

            api.ExecuteOrder("Sell", symbol, sellPrice, remaining);

            Thread.Sleep(secondsToMarket*1000);

            var openPos = api.GetOpenPositionForSymbol(symbol);
            api.CancelSellOrdersForSymbol(symbol);

            if (openPos.Volume != 0)
            {
                ExecuteFlatOrder(symbol);
            }
            else
            {
                Global.RemoveSymbol(symbol);
            }
        }

        public void ExecuteCancelOpenOrders(string side, string symbol, int remaining)
        {
            if (remaining > 0)
            {
                FormHelper.FormSetText(String.Format("{0} shares for {1} is remained to be filled", remaining, symbol));
                log.Updatelog(String.Format("Cancel Open orders, {0} shares for {1} is remained to be filled", remaining, symbol));
                if (side =="Sell")
                    api.CancelSellOrdersForSymbol(symbol);
                else if (side =="Buy")
                    api.CancelBuyOrdersForSymbol(symbol);
                Thread.Sleep(1100);
            }
        }

        public int SafetyCheck(int tries)
        {
            tries--;

            //For Safety reason!!!
            if (tries == 0)
            {
                FormHelper.FormSetText("Something is wrong with the program!");
                FormHelper.FormSetText("Flat all!!!");
                log.Updatelog("Safety exit!!!");
                throw new ApplicationException("Something is wrong!!!");
            }

            return tries;
        }

        public int GetRemainingShareFromOpenPos(OpenPosition openPos)
        {
            var inPositionShare = openPos == null ? 0 : Math.Abs(openPos.Volume);
            return RemainingShare(inPositionShare, Global.ShareSize);
        }

        public int GetRemainingShareForSymbol(EnterType e, string symbol, int initialShareSize)
        {
            var openPos = api.GetOpenPositionForSymbol(symbol);
            if (e == EnterType.Buy)
            {
                if (openPos != null && openPos.Side != "B")
                    api.FlattenSymbol(symbol);
            }
            else if (e== EnterType.Sell)
            {
                if (openPos != null && openPos.Side == "B")
                    api.FlattenSymbol(symbol);
            }

            return GetRemainingShareFromOpenPos(openPos, initialShareSize);
        }

        public int GetRemainingShareFromOpenPos(OpenPosition openPos, int shareSize)
        {
            var inPositionShare = openPos == null ? 0 : Math.Abs(openPos.Volume);
            return RemainingShare(inPositionShare, shareSize);
        }

        private int GetRemainingShareForExit(OpenPosition openPos, int shareSize, int initial)
        {
            var partialOut = (initial - Math.Abs(openPos.Volume));
                     
            var result = shareSize - partialOut;
                        return result;
        }

        // Use this method when lots of call made to get level 1 quote
        public Lv1Quote ExecuteGetMassL1(string symbol)
        {
            var r = new Random();
            var l1 = new Lv1Quote();
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    l1 = api.GetL1(symbol);
                    return l1;
                }
                catch (Exception e)
                {
                }
                Thread.Sleep(r.Next(200, 3000));
            }
            return l1;
        }

        public Lv1Quote ExecuteGetL1 (string symbol)
        {
            var l1 = new Lv1Quote();
            try
            {
                l1 = api.GetL1(symbol);
            }
            catch (Exception e)
            {
                return l1;
            }

            return l1;
        }

        public void CheckOrderStateAndCancelOrder(string execId, string side, string symbol)
        {
            var orderId = api.GetOrderNumber(execId);
            Thread.Sleep(1000);
            var state = api.GetOrderState(orderId);
            if (state.State != State.Cancelled && state.State != State.Filled)
            {
                FormHelper.FormSetText(String.Format("Did not get filled Cancel open orders"));

                if (side == "Buy")
                    api.CancelBuyOrdersForSymbol(symbol);
                else if(side == "Sell")
                    api.CancelSellOrdersForSymbol(symbol);
                Thread.Sleep(1000);
            }
        }

        public void ExecuteFlatOrder(string symbol)
        {
            FormHelper.FormSetText(String.Format("Executing Flat for {0},", symbol));
            api.FlattenSymbol(symbol);
            Global.RemoveSymbol(symbol);
        }

       

        public List<DisplayGrid> GetLv1GainPosition2()
        {
            var openPositionList = Global.Tradelist2.Values.ToList();

            var lv1QuoteList = Global.Tradelist2.Select(item => api.GetL1(item.Value.Name)).ToList();

            var lv1GainPosition = from opl in openPositionList
                                  join lvQ in lv1QuoteList on opl.Name equals lvQ.Symbol
                                  select
                                      new DisplayGrid
                                      {
                                          Symbol = opl.Name,
                                          BidPrice = lvQ.BidPrice,
                                          AskPrice = lvQ.AskPrice,
                                          PositionSize = opl.Volume,
                                          Side = opl.Side,
                                          InPrice = opl.InPrice,
                                          StopLoss = opl.StopLoss,
                                          Gain =
                                              opl.Side == "Buy"
                                                  ? (lvQ.BidPrice - opl.InPrice) * opl.Volume
                                                  : (opl.InPrice - lvQ.AskPrice) * opl.Volume
                                      };

            return lv1GainPosition.ToList();
        }

        public List<Lv1Quote> GetLv1GainPosition()
        {
            var openPositionList = Global.Tradelist2.Values.ToList();

            var lv1QuoteList2 = new List<Lv1Quote>();
            foreach (var item in openPositionList)
            {
                lv1QuoteList2.Add(api.GetL1(item.Name));
            }

            return lv1QuoteList2.ToList();
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof (T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
    }

    public abstract class SiteConfig
    {
        public string Uri
        {
            get { return ConfigurationManager.AppSettings["chrome"]; }
        }
    }

    public enum EnterType
    {
        Buy = 0,
        Sell = 1
    }
}