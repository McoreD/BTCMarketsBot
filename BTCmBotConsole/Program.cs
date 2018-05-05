﻿using BTCMarketsBot;
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
        private const string INSTRUMENT = "BTC"; // What you wanna buy initially e.g. XRP
        private const string CURRENCY = "AUD"; // What you have e.g. AUD

        private const string CONSOLE_WAITING = "Waiting for next execution...";

        private static OrderHistoryData OpenOrdersHistory;
        private static Timer marketTickTimer = new Timer();
        private static Logger BotLogger;

        private static Random rnd = new Random();

        private static void Main(string[] args)
        {
            // Configure Logger
            BotLogger = new Logger(Bot.LogFilePath);
            // BotLogger.WriteLine(",BTC/AUD, ETH/AUD, ETH/BTC");
            BotLogger.WriteLine($",Buy Volume (Unit 1), Unit 1, Balance (Unit 2), Bid/Price (Unit 2), Unit 2, Spend Total (Unit 2), Profit, Sell Volume, Ask/Sell Price");

            // Load settings
            Bot.LoadSettings();

            // configure timer - random between 30 and 60 seconds

            marketTickTimer.Interval = rnd.Next(30, 60) * 1000;
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

        private static void Trade1()
        {
            // Get updated Buy/Sell prices
            Bot.LoadSettings();

            // Settings
            BTCMarketsHelper.ProfitMargin = 6;

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

                    Console.WriteLine($"Best Bid (Buy) {marketData.bestbid} | Best Ask (Sell): {tradingData.BuyPrice}");
                    StatisticsHelper.AddBestBid(marketData.bestbid);

                    Console.WriteLine($"Best Ask (Sell): {marketData.bestAsk} | Sell Target: {tradingData.SellPrice}");
                    StatisticsHelper.AddBestAsk(marketData.bestAsk);

                    Console.WriteLine($"SD BestAsk: {StatisticsHelper.GetSDBestAsk.ToDecimalString(8)}");
                    Console.WriteLine($"Buy Price (2SD Lowerbound): {StatisticsHelper.Get2SDBestBid}");
                    Console.WriteLine($"Sell Price (2SD Upperbound): {StatisticsHelper.Get2SDBestAsk}");

                    // create orders
                    if (tradingData.IsProfitableBuy)
                    {
                        CreateOrderData buyOrder = BTCMarketsHelper.CreateNewOrder(marketData.currency, marketData.instrument,
                            (long)(tradingData.BuyPrice * ApplicationConstants.NUMERIC_MULTIPLIER),
                            (long)(tradingData.BuyVolume * ApplicationConstants.NUMERIC_MULTIPLIER),
                            "Bid", "Limit");

                        Console.WriteLine($"Buy volume ({marketData.instrument}): {tradingData.BuyVolume}");
                        Console.WriteLine($"Buy price ({marketData.currency}): {tradingData.BuyPrice}");
                        Console.WriteLine($"Spend total ({marketData.currency}): {tradingData.SpendTotal}");

                        if (!buyOrder.success) Console.WriteLine(buyOrder.errorMessage);
                    }

                    if (tradingData.IsProfitableSell)
                    {
                        System.Threading.Thread.Sleep(rnd.Next(1, 10) * 1000);
                        CreateOrderData sellOrder = BTCMarketsHelper.CreateNewOrder(marketData.currency, marketData.instrument,
                         (long)(tradingData.SellPrice * ApplicationConstants.NUMERIC_MULTIPLIER),
                        (long)(tradingData.SellVolume * ApplicationConstants.NUMERIC_MULTIPLIER),
                         "Ask", "Limit");

                        Console.WriteLine($"Sell volume ({marketData.instrument}): {tradingData.SellVolume}");
                        Console.WriteLine($"Sell price ({marketData.currency}): {tradingData.SellPrice}");
                        Console.WriteLine($"Receive total ({marketData.currency}): {tradingData.ReceiveTotal}");

                        if (!sellOrder.success) Console.WriteLine(sellOrder.errorMessage);
                    }

                    // append csv line
                    BotLogger.WriteLine($",{tradingData.BuyVolume}, {marketData.instrument}, Balance (Unit 2), " +
                        $"{tradingData.BuyPrice}, {marketData.currency}, {tradingData.SpendTotal}, {BTCMarketsHelper.ProfitMargin}%, " +
                        $"{tradingData.SellVolume}, {tradingData.SellPrice}");
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