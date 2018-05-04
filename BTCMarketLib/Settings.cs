using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public class Settings : SettingsBase<Settings>
    {
        public string APIKey { get; set; }
        public string PrivateKey { get; set; }

        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }

        public int ExchangeTypeIndex { get; set; } = 0;

        public int ProfitMargin { get; set; } = 6;

        public bool ProfitMarginSplit { get; set; } = true; // if true, 10% profit margin is buy low by 5% and sell high by 5%

        public int IntervalIndex { get; set; } = 0;
    }
}