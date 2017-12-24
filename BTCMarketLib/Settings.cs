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
        public int ExchangeTypeIndex { get; set; } = 0;

        public int ProfitMarginIndex { get; set; } = 1;

        public bool ProfitMarginSplit { get; set; } = true;

        public int IntervalIndex { get; set; } = 0;

        public double TradingFee { get; set; } = 0.22;
    }
}