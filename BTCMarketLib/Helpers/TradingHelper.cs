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
        public static TradingData GetTradingData(MarketTickData marketData, bool splitProfitMargin)
        {
            double balance = BTCMarketsHelper.RetrieveAccountBalance(marketData.currency) * 0.4; // buying currency

            TradingFeeData feeData = JsonConvert.DeserializeObject<TradingFeeData>(BTCMarketsHelper.SendRequest(MethodConstants.TRADING_FEE_PATH(marketData.instrument, marketData.currency), null));
            double tradingFeeMultiplier = 1 + TradingFeeData.GetTradingFee(feeData);

            double buyVolume = Math.Round((balance / 100000000.0) / (marketData.bestAsk * tradingFeeMultiplier), 8);

            return GetTradingData(marketData, splitProfitMargin, buyVolume, feeData);
        }

        public static TradingData GetTradingData(MarketTickData marketData, bool splitProfitMargin, double buyVolume, TradingFeeData feeData = null)
        {
            TradingData tradingData = new TradingData();

            double profitMargin = splitProfitMargin ? BTCMarketsHelper.ProfitMargin / 2 : BTCMarketsHelper.ProfitMargin;

            double profitMultiplier = profitMargin / 100.0 + 1.0;

            tradingData.BuyVolume = buyVolume;

            double buyPrice = marketData.bestAsk;

            buyPrice = Math.Round(splitProfitMargin ? buyPrice * (1 - profitMargin / 100.0) : buyPrice, 8);

            tradingData.BuyPrice = buyPrice;

            if (feeData == null)
                feeData = JsonConvert.DeserializeObject<TradingFeeData>(BTCMarketsHelper.SendRequest(MethodConstants.TRADING_FEE_PATH(marketData.instrument, marketData.currency), null));

            double tradingFeeMultiplier = 1 + TradingFeeData.GetTradingFee(feeData); // Bot.Settings.TradingFee / 100.0 + 1;

            tradingData.SpendTotal = Math.Round(tradingData.BuyVolume * buyPrice * tradingFeeMultiplier, 8);

            tradingData.SellPrice = Math.Round(buyPrice * profitMultiplier, 8);

            tradingData.SellVolume = Math.Round(tradingData.SpendTotal / tradingData.SellPrice, 8);

            return tradingData;
        }
    }

    public class TradingData
    {
        public double BuyVolume { get; set; }
        public double BuyPrice { get; set; }
        public double SellVolume { get; set; }
        public double SellPrice { get; set; }

        public double SpendTotal { get; set; }
    }
}