using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public static class TradingHelper
    {
        public static double BuyVolumeInput;

        public static TradingData GetTradingData()
        {
            MarketTickData marketData = BTCMarketsHelper.MarketTickData;

            TradingData tradingData = new TradingData();

            double profitMultiplier = BTCMarketsHelper.ProfitMargin / 100.0 + 1.0;

            tradingData.BuyVolume = BuyVolumeInput;

            double buyPrice;
            double.TryParse(marketData.bestAsk, out buyPrice);

            tradingData.BuyPrice = buyPrice;

            double tradingFees = App.Settings.TradingFee / 100.0 + 1;

            double spendTotal = tradingData.BuyVolume * buyPrice * tradingFees;

            tradingData.SellPrice = Math.Round(buyPrice * profitMultiplier, 8);

            tradingData.SellVolume = Math.Round(spendTotal / tradingData.SellPrice, 8);

            return tradingData;
        }
    }

    public class TradingData
    {
        public double BuyVolume { get; set; }
        public double BuyPrice { get; set; }
        public double SellVolume { get; set; }
        public double SellPrice { get; set; }
    }
}