using RestSharp;
using RestSharp.Deserializers;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public class BTCMarketsHelper
    {
        internal static MarketTickData GetMarketTick()
        {
            return JsonHelpers.DeserializeFromString<MarketTickData>(SendRequest(MethodConstants.MARKET_TICK_PATH, null));
        }

        /// <summary>
        ///     This method constructs the core parts used for the request to BTC markets
        /// </summary>
        /// <param name="action">The API Action that is being requested e.g "\account\balance"</param>
        /// <param name="postData">The POST data that forms part of the request (null for a GET request)</param>
        /// <returns>returns the string output from the content of the REST response</returns>
        public static string SendRequest(string action, string postData)
        {
            var response = "";
            try
            {
                //get the epoch timestamp to be used as the nonce
                var timestamp = ConversionHelper.ReturnCurrentTimeStampInMilliseconds();

                // create the string that needs to be signed
                var stringToSign = BuildStringToSign(action, postData, timestamp);

                // build signature to be included in the http header
                var signature = SecurityHelper.ComputeHash(ApplicationConstants.PRIVATE_KEY, stringToSign);

                response = Query(postData, action, signature, timestamp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return response;
        }

        /// <summary>
        ///     Buils up the string that will be signed and used as part of the request
        /// </summary>
        /// <param name="action">The API Action that is being requested e.g "\account\balance"</param>
        /// <param name="postData">Any data to be posted with the request - will be in a JSON format</param>
        /// <param name="timestamp">The epoch timestamp that will be used as the nonce for the signed string</param>
        /// <returns>The string to be signed and passed through with the request</returns>
        private static string BuildStringToSign(string action, string postData,
            string timestamp)
        {
            var stringToSign = new StringBuilder();
            stringToSign.Append(action + "\n");
            stringToSign.Append(timestamp + "\n");
            if (postData != null)
            {
                stringToSign.Append(postData);
            }

            return stringToSign.ToString();
        }

        /// <summary>
        ///     Uses the RestSharp library to generate the request (POST or GET by default) to BTC Markets.
        ///     I used this library for ease of use and cleanliness
        ///     but you can craft and submit the request any way you like using these parameters.
        /// </summary>
        /// <param name="data">Any data to be passed through for a POST request, will be in JSON format</param>
        /// <param name="action">The API action that is being requested on the API</param>
        /// <param name="signature">The signed string</param>
        /// <param name="timestamp">
        ///     The generated timestamp for the request - must be recieved by BTC within 30 seconds or the
        ///     request will be refused
        /// </param>
        /// <returns>The response from the BTC Markets API</returns>
        public static string Query(string data, string action, string signature, string timestamp)
        {
            var client = new RestClient(ApplicationConstants.BASEURL);

            var request = new RestRequest(action);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
            request.Method = data != null ? Method.POST : Method.GET;

            request = BuildRequestHeaders(request, signature, timestamp);

            if (data != null)
            {
                request.AddParameter("application/json", data, ParameterType.RequestBody);
            }

            var queryResult = client.Execute(request);

            return queryResult.Content;
        }

        /// <summary>
        ///     Builds the Default Headers and Parameters that are required by the BTC API
        /// </summary>
        /// <param name="btcRequest">the RestRequest object to be sent</param>
        /// <param name="signature">The signed string for the required request</param>
        /// <param name="timestamp">The timestamp passed with the request</param>
        /// <returns></returns>
        private static RestRequest BuildRequestHeaders(RestRequest btcRequest, string signature, string timestamp)
        {
            btcRequest.AddHeader("Accept", HeaderConstants.CONTENT);
            btcRequest.AddHeader("Accept-Charset", HeaderConstants.ENCODING);
            btcRequest.AddHeader("Content-Type", HeaderConstants.CONTENT);
            btcRequest.AddHeader(HeaderConstants.APIKEY_HEADER, ApplicationConstants.API_KEY);
            btcRequest.AddHeader(HeaderConstants.SIGNATURE_HEADER, signature);
            btcRequest.AddHeader(HeaderConstants.TIMESTAMP_HEADER, timestamp);

            return btcRequest;
        }
    }

    public class MarketTickData
    {
        public string bestbid { get; set; }
        public string bestAsk { get; set; }
        public string lastPrice { get; set; }
        public string currency { get; set; }
        public string instrument { get; set; }
        public string timestamp { get; set; }
    }

    public class MartketOrderBookData
    {
        public string currency { get; set; }
        public string instrument { get; set; }
        public string timestamp { get; set; }
        public string[] asks { get; set; }
        public string[] bids { get; set; }
    }
}