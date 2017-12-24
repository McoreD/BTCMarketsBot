using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    /// <summary>
    /// Limits are from https://github.com/BTCMarkets/API/wiki/faq
    /// Lower limit: 10 calls per 10 seconds
    /// </summary>
    public static class APICallLimiter
    {
        internal static List<double> listQueryTimestamps = new List<double>();

        public static void QueueRequest(double secondsSinceEpoch)
        {
            if (listQueryTimestamps.Count > 1)
            {
                double waitSeconds = secondsSinceEpoch - listQueryTimestamps[listQueryTimestamps.Count - 1];
                if (waitSeconds < 2000)
                    Thread.Sleep(1000);
            }

            listQueryTimestamps.Add(secondsSinceEpoch);
            Console.WriteLine($"Query ID: {listQueryTimestamps.Count} {secondsSinceEpoch}");

            if (listQueryTimestamps.Count > 100)
                listQueryTimestamps.Clear();
        }
    }
}