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
        private const string INSTRUMENT = "XRP"; // What you wanna buy initially e.g. XRP
        private const string CURRENCY = "AUD"; // What you have e.g. AUD

        private const string CONSOLE_WAITING = "Waiting for next execution...";

        private static OrderHistoryData OpenOrdersHistory;
        private static Timer marketTickTimer = new Timer();
        private static Logger BotLogger;

        private static decimal lastETH_BTC = 0;
        private static decimal lastBTC_Volume = 0;
        private static decimal lastETH_Volume = 0;

        private static Random rnd = new Random();

        private static void Main(string[] args)
        {
            // Configure Logger
            BotLogger = new Logger(Bot.LogFilePath);
            // BotLogger.WriteLine(",BTC/AUD, ETH/AUD, ETH/BTC");
            BotLogger.WriteLine($",Buy Volume (Unit 1), Unit 1, Balance (Unit 2), Bid/Price (Unit 2), Unit 2, Spend Total (Unit 2), Profit, Sell Volume, Ask/Sell Price");

            // Read API_KEY and PRIVATE_KEY
            Bot.ReadAPIKeys();

            // Load settings
            Bot.LoadSettings();

            // configure timer - random between 30 and 60 seconds

            marketTickTimer.Interval = rnd.Next(60, 240) * 1000;
            marketTickTimer.Elapsed += MarketTickTimer_Tick;
            marketTickTimer.Start();

            // Console.Write("Input buy volume (ETH): ");
            // double.TryParse(Console.ReadLine().ToString(), out BuyVolume);

            Console.WriteLine(CONSOLE_WAITING);
            Console.ReadLine();
        }

        private static void MarketTickTimer_Tick(object sender, EventArgs e)
        {
            Trade1();
        }

        private static void Trade3()
        {
            // Settings
            BTCMarketsHelper.ProfitMargin = 0;

            // Get number of Open Orders
            OpenOrdersHistory = BTCMarketsHelper.OrderOpen(CURRENCY, INSTRUMENT, 10, "1");

            if (OpenOrdersHistory.success && OpenOrdersHistory.orders != null)
            {
                if (OpenOrdersHistory.orders.Length > 1)
                {
                    Console.WriteLine("Open orders are still active...");
                }
                else
                {
                    // get ETH/BTC market data i.e. instrument/currency
                    Console.WriteLine($"Previous Ask Price: {lastETH_BTC}");
                    MarketTickData marketData = BTCMarketsHelper.GetMarketTick($"{INSTRUMENT}/{CURRENCY}");
                    Console.WriteLine($"Current Ask Price: {marketData.bestAsk}");
                    lastETH_BTC = marketData.bestAsk;

                    // get trading data
                    TradingData tradingData = TradingHelper.GetTradingData(marketData);

                    if (lastETH_BTC > 0) // ensure first round is skipped when there is no history
                    {
                        if (marketData.bestAsk > lastETH_BTC)
                        {
                            Console.WriteLine($"Previous Volume (BTC): {lastBTC_Volume}");
                            if (tradingData.BuyVolume > lastBTC_Volume)
                            {
                                Console.WriteLine("ETH has gone stronger. Sell ETH.");
                            }
                        }
                        else if (marketData.bestAsk < lastETH_BTC)
                        {
                            Console.WriteLine("BTC has gone stronger. Buy ETH.");
                            Console.WriteLine($"Previous Buy Volume (ETH): {lastETH_Volume}");
                            Console.WriteLine($"Current Buy volume ({marketData.instrument}): {tradingData.BuyVolume}");
                        }
                    }

                    Console.WriteLine($"Buy price ({marketData.currency}): {tradingData.BuyPrice}");
                    Console.WriteLine($"Sell volume ({marketData.instrument}): {tradingData.SellVolume}");
                    Console.WriteLine($"Sell price ({marketData.currency}): {tradingData.SellPrice}");
                    Console.WriteLine($"Spend total ({marketData.currency}): {tradingData.SpendTotal}");

                    /*
                    // create orders
                    CreateOrderData buyOrder = BTCMarketsHelper.CreateNewOrder(marketData.currency, marketData.instrument,
                        (long)(tradingData.BuyPrice * ApplicationConstants.NUMERIC_MULTIPLIER),
                        (int)(tradingData.BuyVolume * ApplicationConstants.NUMERIC_MULTIPLIER),
                        "Bid", "Limit");

                    if (buyOrder.success)
                    {
                        CreateOrderData sellOrder = BTCMarketsHelper.CreateNewOrder(marketData.currency, marketData.instrument,
                         (long)(tradingData.SellPrice * ApplicationConstants.NUMERIC_MULTIPLIER),
                        (int)(tradingData.SellVolume * ApplicationConstants.NUMERIC_MULTIPLIER),
                         "Ask", "Limit");

                        if (sellOrder.success)
                        {
                            // append csv line
                            BotLogger.WriteLine($",{tradingData.BuyVolume.ToDecimalString(8)}, {marketData.instrument}, Balance (Unit 2), " +
                                $"{tradingData.BuyPrice.ToDecimalString(8)}, {marketData.currency}, {tradingData.SpendTotal.ToDecimalString(8)}, {BTCMarketsHelper.ProfitMargin}%, " +
                                $"{tradingData.SellVolume.ToDecimalString(8)}, {tradingData.SellPrice.ToDecimalString(8)}");
                        }
                        else
                        {
                            Console.WriteLine(sellOrder.errorMessage);
                        }
                    }
                    else
                    {
                        Console.WriteLine(buyOrder.errorMessage);
                    }
                    */
                }
            }
            else
            {
                Console.WriteLine(OpenOrdersHistory.errorMessage);
            }

            Console.WriteLine(CONSOLE_WAITING);
            Console.WriteLine();
        }

        private static void Trade2()
        {
            // get BTC balance

            // higest growth rate curr buys lowest growth rate
            MarketTickData mdBTC_AUD = BTCMarketsHelper.GetMarketTick($"BTC/AUD");
            MarketTickData mdETH_AUD = BTCMarketsHelper.GetMarketTick($"ETH/AUD");
            MarketTickData mdETH_BTC = BTCMarketsHelper.GetMarketTick($"ETH/BTC");

            BotLogger.WriteLine($",{mdBTC_AUD.bestAsk}, {mdETH_AUD.bestAsk}, {mdETH_BTC.bestAsk}");
        }

        private static void Trade1()
        {
            // Settings
            BTCMarketsHelper.ProfitMargin = 2;

            // Get number of Open Orders
            OpenOrdersHistory = BTCMarketsHelper.OrderOpen(CURRENCY, INSTRUMENT, 10, "1");

            if (OpenOrdersHistory.success && OpenOrdersHistory.orders != null)
            {
                if (OpenOrdersHistory.orders.Length > 1)
                {
                    Console.WriteLine("Open orders are still active...");
                }
                else
                {
                    // get ETH/BTC market data i.e. instrument/currency
                    MarketTickData marketData = BTCMarketsHelper.GetMarketTick($"{INSTRUMENT}/{CURRENCY}");

                    // get trading data
                    TradingData tradingData = TradingHelper.GetTradingData(marketData, splitProfitMargin: true);

                    Console.WriteLine($"Buy volume ({marketData.instrument}): {tradingData.BuyVolume}");
                    Console.WriteLine($"Buy price ({marketData.currency}): {tradingData.BuyPrice}");
                    Console.WriteLine($"Sell volume ({marketData.instrument}): {tradingData.SellVolume}");
                    Console.WriteLine($"Sell price ({marketData.currency}): {tradingData.SellPrice}");
                    Console.WriteLine($"Spend total ({marketData.currency}): {tradingData.SpendTotal}");

                    // create orders
                    CreateOrderData buyOrder = BTCMarketsHelper.CreateNewOrder(marketData.currency, marketData.instrument,
                        (long)(tradingData.BuyPrice * ApplicationConstants.NUMERIC_MULTIPLIER),
                        (long)(tradingData.BuyVolume * ApplicationConstants.NUMERIC_MULTIPLIER),
                        "Bid", "Limit");

                    if (buyOrder.success)
                    {
                        System.Threading.Thread.Sleep(rnd.Next(1, 10) * 1000);
                        CreateOrderData sellOrder = BTCMarketsHelper.CreateNewOrder(marketData.currency, marketData.instrument,
                         (long)(tradingData.SellPrice * ApplicationConstants.NUMERIC_MULTIPLIER),
                        (long)(tradingData.SellVolume * ApplicationConstants.NUMERIC_MULTIPLIER),
                         "Ask", "Limit");

                        if (sellOrder.success)
                        {
                            // append csv line
                            BotLogger.WriteLine($",{tradingData.BuyVolume}, {marketData.instrument}, Balance (Unit 2), " +
                                $"{tradingData.BuyPrice}, {marketData.currency}, {tradingData.SpendTotal}, {BTCMarketsHelper.ProfitMargin}%, " +
                                $"{tradingData.SellVolume}, {tradingData.SellPrice}");
                        }
                        else
                        {
                            Console.WriteLine(sellOrder.errorMessage);
                        }
                    }
                    else
                    {
                        Console.WriteLine(buyOrder.errorMessage);
                    }
                }
            }
            else
            {
                Console.WriteLine(OpenOrdersHistory.errorMessage);
            }

            Console.WriteLine(CONSOLE_WAITING);
            Console.WriteLine();
        }
    }
}