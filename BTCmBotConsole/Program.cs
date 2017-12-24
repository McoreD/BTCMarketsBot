using BTCMarketsBot;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace BTCmBotConsole
{
    internal class Program
    {
        private static double BuyVolume = 0.0;
        private static int TradeID = 1;
        private static Timer marketTickTimer = new Timer();

        private static void Main(string[] args)
        {
            // Read API_KEY and PRIVATE_KEY
            Bot.ReadAPIKeys();

            // Load settings
            Bot.LoadSettings();
            Bot.Settings.ProfitMarginSplit = false;

            // configure timer - random between 30 and 60 seconds
            Random rnd = new Random();
            marketTickTimer.Interval = rnd.Next(30, 60) * 1000;
            marketTickTimer.Elapsed += MarketTickTimer_Tick;
            marketTickTimer.Start();

            BTCMarketsHelper.ProfitMargin = 10;

            Console.Write("Input buy volume (ETH): ");
            double.TryParse(Console.ReadLine().ToString(), out BuyVolume);

            Console.WriteLine("Waiting for next execution...");
            Console.ReadLine();
        }

        private static void MarketTickTimer_Tick(object sender, EventArgs e)
        {
       

            // get BTC/ETH market data
            MarketTickData marketData = BTCMarketsHelper.GetMarketTick("ETH/BTC");

            // get trading data
            TradingData tradingData = TradingHelper.GetTradingData(marketData, BuyVolume);

            Console.WriteLine($"Buy price (BTC): {tradingData.BuyPrice.ToDecimalString(8)}");
            Console.WriteLine($"Sell volume (ETH): {tradingData.SellVolume.ToDecimalString(8)}");
            Console.WriteLine($"Sell price (BTC): {tradingData.BuyPrice.ToDecimalString(8)}");
            Console.WriteLine($"Spend total (BTC): {tradingData.SpendTotal.ToDecimalString(8)}");


            Console.WriteLine("Waiting for next execution...");
            Console.WriteLine();

        }
    }
}