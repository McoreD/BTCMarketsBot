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
            decimal balance = BTCMarketsHelper.RetrieveAccountBalance(marketData.currency) / 2; // buying currency

            TradingFeeData feeData = JsonConvert.DeserializeObject<TradingFeeData>(BTCMarketsHelper.SendRequest(MethodConstants.TRADING_FEE_PATH(marketData.instrument, marketData.currency), null));
            decimal tradingFeeMultiplier = 1 + TradingFeeData.GetTradingFee(feeData);

            decimal buyVolume = Math.Round((balance / 100000000) / (marketData.bestAsk * tradingFeeMultiplier), 8); // maximum buy volume

            return GetTradingData(marketData, splitProfitMargin, buyVolume, feeData);
        }

        public static TradingData GetTradingData(MarketTickData marketData, bool splitProfitMargin, decimal buyVolume, TradingFeeData feeData = null)
        {
            TradingData tradingData = new TradingData();

            decimal profitMargin = splitProfitMargin ? BTCMarketsHelper.ProfitMargin / 2 : BTCMarketsHelper.ProfitMargin;

            decimal profitMultiplier = profitMargin / 100 + 1;

            tradingData.BuyVolume = buyVolume;

            decimal buyPrice = marketData.bestAsk;

            int roundPos = marketData.currency == "AUD" ? 2 : 8;
            buyPrice = Math.Round(splitProfitMargin ? buyPrice * (1 - profitMargin / 100) : buyPrice, roundPos);

            tradingData.BuyPrice = buyPrice;

            if (feeData == null)
                feeData = JsonConvert.DeserializeObject<TradingFeeData>(BTCMarketsHelper.SendRequest(MethodConstants.TRADING_FEE_PATH(marketData.instrument, marketData.currency), null));

            decimal tradingFeeMultiplier = 1 + TradingFeeData.GetTradingFee(feeData); // Bot.Settings.TradingFee / 100.0 + 1;

            tradingData.SpendTotal = Math.Round(tradingData.BuyVolume * buyPrice * tradingFeeMultiplier, roundPos);

            tradingData.SellPrice = Math.Round(marketData.bestbid * profitMultiplier, roundPos);

            tradingData.SellVolume = Math.Round(tradingData.SpendTotal / tradingData.SellPrice, 8);

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
    }
}