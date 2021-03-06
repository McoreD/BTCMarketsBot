﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public class MarketTickData
    {
        public decimal bestbid { get; set; }
        public decimal bestAsk { get; set; }
        public decimal lastPrice { get; set; }
        public string currency { get; set; }
        public string instrument { get; set; }
        public double timestamp { get; set; }
    }

    public class MartketOrderBookData
    {
        public string currency { get; set; }
        public string instrument { get; set; }
        public double timestamp { get; set; }
        public string[] asks { get; set; }
        public string[] bids { get; set; }
    }

    public class OrderHistoryData
    {
        public bool success { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public OrderData[] orders { get; set; }
    }

    public class OrderData
    {
        public int id { get; set; }
        public string currency { get; set; }
        public string instrument { get; set; }
        public string orderSide { get; set; }
        public string ordertype { get; set; }
        public double creationTime { get; set; }
        public string status { get; set; }
        public string errorMessage { get; set; }
        public double price { get; set; }
        public double volume { get; set; }
        public double openVolume { get; set; }
        public string clientRequestId { get; set; }
        public OrderTradesData[] trades { get; set; }
    }

    public class OrderTradesData
    {
        public int id { get; set; }
        public double creationTime { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public double volume { get; set; }
        public string side { get; set; }
        public double fee { get; set; }
        public int orderIdd { get; set; }
    }

    public class BalanceData
    {
        private BalanceItem[] items;

        public BalanceData(BalanceItem[] items)
        {
            this.items = items;
        }

        public decimal GetAvailableBalance(string currency)
        {
            decimal balance = 0;

            foreach (BalanceItem item in items)
            {
                if (item.currency.Equals(currency))
                {
                    balance = item.balance - item.pendingFunds;
                }
            }

            return balance;
        }
    }

    public class BalanceItem
    {
        public decimal balance { get; set; }
        public decimal pendingFunds { get; set; }
        public string currency { get; set; }
    }

    public class TradingFeeData
    {
        public bool success { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public decimal tradingFeeRate { get; set; }
        public double volume30Day { get; set; }

        public static decimal GetTradingFee(TradingFeeData data)
        {
            return data.tradingFeeRate / 100000000;
        }
    }

    public class CreateOrderData
    {
        public bool success { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public int id { get; set; }
        public string clientRequestId { get; set; }
    }
}