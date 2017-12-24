using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public class ApplicationConstants
    {
        public static string API_KEY { get; set; }

        public static string PRIVATE_KEY { get; set; }

        public const string BASEURL = "https://api.btcmarkets.net";

        public const double NUMERIC_MULTIPLIER = 100000000.0;
    }
}