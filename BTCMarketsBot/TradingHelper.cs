using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public static class TradingHelper
    {
        public static TradingData GetTradingData(MarketTickData marketData, double buyVolume)
        {
            TradingData tradingData = new TradingData();

            double profitMargin = App.Settings.ProfitMarginSplit ? BTCMarketsHelper.ProfitMargin / 2 : BTCMarketsHelper.ProfitMargin;

            double profitMultiplier = profitMargin / 100.0 + 1.0;

            tradingData.BuyVolume = buyVolume;

            double buyPrice;

            double.TryParse(marketData.bestAsk, out buyPrice);

            buyPrice = App.Settings.ProfitMarginSplit ? buyPrice * (1 - profitMargin / 100.0) : buyPrice;

            tradingData.BuyPrice = buyPrice;

            double tradingFees = App.Settings.TradingFee / 100.0 + 1;

            tradingData.SpendTotal = tradingData.BuyVolume * buyPrice * tradingFees;

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