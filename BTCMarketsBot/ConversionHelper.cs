using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public class ConversionHelper
    {
        public static string ReturnCurrentTimeStampInMilliseconds()
        {
            var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var secondsSinceEpoch = (long)(DateTime.UtcNow - unixTime).TotalMilliseconds;

            APICallLimiter.QueueRequest(secondsSinceEpoch);

            var nonce = secondsSinceEpoch.ToString();

            return nonce;
        }

        public static DateTime TimeStampInDateFormat(string milliseconds)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(Convert.ToDouble(milliseconds));
        }
    }
}