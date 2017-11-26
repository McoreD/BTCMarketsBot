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
        public string ExchangeType { get; set; }
    }
}