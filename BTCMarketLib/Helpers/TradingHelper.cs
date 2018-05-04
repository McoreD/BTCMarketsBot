using Newtonsoft.Json;
using System;

namespace BTCMarketsBot
{
    public static class TradingHelper
    {
        /// <summary>
        /// Trading data with buy volume calculated as maximum
        /// </summary>
        /// <param name="marketData"></param>
        /// <returns></returns>
        public static TradingData GetTradingData(MarketTickData marketData, bool splitProfitMargin = false)
        {
            decimal balance = BTCMarketsHelper.RetrieveAccountBalance(marketData.currency); // buying currency

            TradingFeeData feeData = JsonConvert.DeserializeObject<TradingFeeData>(BTCMarketsHelper.SendRequest(MethodConstants.TRADING_FEE_PATH(marketData.instrument, marketData.currency), null));
            decimal tradingFeeMultiplier = 1 + TradingFeeData.GetTradingFee(feeData);

            decimal buyVolume = Math.Round(Math.Floor(balance / 100000000) / (Bot.Settings.BuyPrice * tradingFeeMultiplier), 8); // maximum buy volume

            return GetTradingData(marketData, splitProfitMargin, buyVolume, feeData);
        }

        public static TradingData GetTradingData(MarketTickData marketData, bool splitProfitMargin, decimal buyVolume, TradingFeeData feeData = null)
        {
            TradingData tradingData = new TradingData();

            tradingData.BuyVolume = buyVolume;

            decimal tradingFee = TradingFeeData.GetTradingFee(feeData);
            decimal tradingFeeMultiplier = 1 + tradingFee; // Bot.Settings.TradingFee / 100.0 + 1;

            int roundPos = marketData.currency == "AUD" ? 2 : 8;

            tradingData.BuyPrice = Math.Round(Bot.Settings.BuyPrice, roundPos);

            tradingData.SpendTotal = Math.Round(tradingData.BuyVolume * tradingData.BuyPrice * tradingFeeMultiplier, roundPos);

            tradingData.SellPrice = Math.Round(Bot.Settings.SellPrice, roundPos);

            tradingData.SellVolume = Math.Round(tradingData.SpendTotal / tradingData.SellPrice, 8);

            tradingData.ReceiveTotal = Math.Round(tradingData.SellPrice * tradingData.SellVolume, roundPos);

            double profitMultiplier = (double)(BTCMarketsHelper.ProfitMargin + 100.0) / 100.0;
            tradingData.IsProfitableBuy = Bot.Settings.BuyPrice < Math.Round(marketData.bestAsk / (decimal)profitMultiplier / tradingFeeMultiplier, roundPos);
            tradingData.IsProfitableSell = Bot.Settings.SellPrice > Math.Round(marketData.bestbid * (decimal)profitMultiplier * tradingFeeMultiplier, roundPos);

            return tradingData;
        }
    }

    public class TradingData
    {
        public decimal BuyVolume { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellVolume { get; set; }
        public decimal SellPrice { get; set; }

        public decimal SpendTotal { get; set; }
        public decimal ReceiveTotal { get; set; }

        public bool IsProfitableBuy { get; set; }
        public bool IsProfitableSell { get; set; }
    }
}