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
        private const string INSTRUMENT = "ETH";
        private const string CURRENCY = "BTC";

        private const string CONSOLE_WAITING = "Waiting for next execution...";

        private static OrderHistoryData OpenOrdersHistory;
        private static Timer marketTickTimer = new Timer();

        private static void Main(string[] args)
        {
            // Read API_KEY and PRIVATE_KEY
            Bot.ReadAPIKeys();

            // Load settings
            Bot.LoadSettings();
            Bot.Settings.ProfitMarginSplit = true;

            // configure timer - random between 30 and 60 seconds
            Random rnd = new Random();
            marketTickTimer.Interval = rnd.Next(300, 600) * 1000;
            marketTickTimer.Elapsed += MarketTickTimer_Tick;
            marketTickTimer.Start();

            BTCMarketsHelper.ProfitMargin = 8;

            // Console.Write("Input buy volume (ETH): ");
            // double.TryParse(Console.ReadLine().ToString(), out BuyVolume);

            Console.WriteLine(CONSOLE_WAITING);
            Console.ReadLine();
        }

        private static void MarketTickTimer_Tick(object sender, EventArgs e)
        {
            // Get number of Open Orders
            OpenOrdersHistory = BTCMarketsHelper.OrderOpen(CURRENCY, INSTRUMENT, 10, "1");

            if (OpenOrdersHistory.orders.Length > 0)
            {
                Console.WriteLine("Open orders are still active...");
            }
            else
            {
                // get ETH/BTC market data i.e. instrument/currency
                MarketTickData marketData = BTCMarketsHelper.GetMarketTick($"{INSTRUMENT}/{CURRENCY}");

                // get trading data
                TradingData tradingData = TradingHelper.GetTradingData(marketData);

                Console.WriteLine($"Buy price ({marketData.currency}): {tradingData.BuyPrice}");
                Console.WriteLine($"Sell volume ({marketData.instrument}): {tradingData.SellVolume}");
                Console.WriteLine($"Sell price ({marketData.currency}): {tradingData.SellPrice}");
                Console.WriteLine($"Spend total ({marketData.currency}): {tradingData.SpendTotal.ToDecimalString(8)}");

                // create order
                CreateOrderData buyOrder = JsonHelpers.DeserializeFromString<CreateOrderData>(BTCMarketsHelper.CreateNewOrder(marketData.currency, marketData.instrument,
                    (long)(tradingData.BuyPrice * ApplicationConstants.NUMERIC_MULTIPLIER),
                    (int)(tradingData.BuyVolume * ApplicationConstants.NUMERIC_MULTIPLIER),
                    "Bid", "Limit"));

                if (buyOrder.success)
                {
                    CreateOrderData sellOrder = JsonHelpers.DeserializeFromString<CreateOrderData>(BTCMarketsHelper.CreateNewOrder(marketData.currency, marketData.instrument,
                     (long)(tradingData.SellPrice * ApplicationConstants.NUMERIC_MULTIPLIER),
                    (int)(tradingData.SellVolume * ApplicationConstants.NUMERIC_MULTIPLIER),
                     "Ask", "Limit"));
                }
                else
                {
                    Console.WriteLine(buyOrder.errorMessage);
                }
            }

            Console.WriteLine(CONSOLE_WAITING);
            Console.WriteLine();
        }
    }
}