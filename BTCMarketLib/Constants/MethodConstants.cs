﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public class MethodConstants
    {
        public const string ACTION_ORDERBOOK = "/orderbook";
        public const string ORDER_CREATE_PATH = "/order/create";
        public const string ORDER_HISTORY_PATH = "/order/history";
        public const string ORDER_OPEN_PATH = "/order/open";
        public const string ORDER_OPEN_DETAIL = "/order/detail";
        public const string ORDER_OPEN_CANCEL = "/order/cancel";
        public const string ORDER_TRADE_HISTORY_PATH = "/order/trade/history";
        public const string ACCOUNT_BALANCE_PATH = "/account/balance";
        public const string MARKET_ORDERBOOK_PATH = "/market/BTC/AUD/orderbook";
        // public const string MARKET_TICK_PATH = "/market/BTC/AUD/tick";
        public const string MARKET_TRADES_PATH = "/market/BTC/AUD/trades";

        /// <summary>
        ///
        /// </summary>
        /// <param name="strExchangeType">ComboBox value e.g. BTC/AUD</param>
        /// <returns>e.g. /market/BTC/AUD/tick</returns>
        public static string MARKET_TICK_PATH
        {
            get { return $"/market/{BTCMarketsHelper.ExchangeType}/tick"; }
        }

        public static string TRADING_FEE_PATH(string instrument, string currency)
        {
            return $"/account/{instrument}/{currency}/tradingfee";
        }
    }
}