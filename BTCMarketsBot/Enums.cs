using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public enum ExchangeType
    {
        [Description("BTC/AUD")]
        BTC_AUD,

        [Description("BCH/BTC")]
        BCH_BTC,

        [Description("ETH/BTC")]
        ETH_BTC,

        [Description("XRP/BTC")]
        XRP_BTC,

        [Description("LTC/BTC")]
        LTC_BTC,

        [Description("ETC/BTC")]
        ETC_BTC
    }
}