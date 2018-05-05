using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public static class StatisticsHelper
    {
        private static List<double> BestBids = new List<double>();
        private static List<double> BestAsks = new List<double>();

        public static void AddBestBid(decimal bestBid)
        {
            BestBids.Add((double)bestBid);
            if (BestBids.Count == 100) BestBids.RemoveAt(0);
        }

        public static void AddBestAsk(decimal bestAsk)
        {
            BestAsks.Add((double)bestAsk);
            if (BestAsks.Count == 100) BestAsks.RemoveAt(0);
        }

        public static double GetMeanBestBid
        {
            get
            {
                return BestBids.Mean();
            }
        }

        public static double GetSDBestBid
        {
            get
            {
                return BestBids.StandardDeviation();
            }
        }

        public static double GetMeanBestAsk
        {
            get
            {
                return BestAsks.Mean();
            }
        }

        public static double GetSDBestAsk
        {
            get
            {
                return BestAsks.StandardDeviation();
            }
        }

        // Mean - 2 * SD because we are after a lower buy price
        public static double Get2SDBestBid
        {
            get
            {
                return GetMeanBestBid - 2 * GetSDBestBid;
            }
        }

        // Mean + 2 * SD because we are after a higher sell price
        public static double Get2SDBestAsk
        {
            get
            {
                return GetMeanBestAsk + 2 * GetSDBestAsk;
            }
        }
    }
}