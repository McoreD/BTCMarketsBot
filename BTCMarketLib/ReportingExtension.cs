using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public static class ReportingExtensions
    {
        public static string ForReporting(this double number)
        {
            string format = "0";
            format += "." + new string('0', 8);
            return (number / 100000000.0).ToString(format);
        }
    }
}