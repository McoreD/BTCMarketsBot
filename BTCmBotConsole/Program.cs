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
        private static int TradeID = 1;
        private static Timer marketTickTimer = new Timer();

        private static void Main(string[] args)
        {
            // Read API_KEY and PRIVATE_KEY
            ReadAPIKeys();

            // configure timer - random between 30 and 60 seconds
            Random rnd = new Random();
            marketTickTimer.Interval = rnd.Next(30, 60) * 1000;
            marketTickTimer.Elapsed += MarketTickTimer_Tick;
            marketTickTimer.Start();

            Bot.LoadSettings();
            BTCMarketsHelper.ProfitMargin = 10;

            Console.ReadKey();
        }

        private static void ReadAPIKeys()
        {
            string fp = Path.Combine(Bot.PersonalFolder, "BTCMarketsAuth.txt");
            if (File.Exists(fp))
            {
                using (StreamReader sr = new StreamReader(fp))
                {
                    ApplicationConstants.API_KEY = sr.ReadLine();
                    ApplicationConstants.PRIVATE_KEY = sr.ReadLine();
                    sr.Close();
                }
            }
        }

        private static void MarketTickTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine($"Trade ID: {TradeID++}");
            // get BTC/ETH
            MarketTickData marketData = BTCMarketsHelper.GetMarketTick("ETH/BTC");
            TradingData tradingData = TradingHelper.GetTradingData(marketData, 0.35);
            Console.WriteLine(tradingData.BuyPrice.ToDecimalString(8));
        }
    }
}