﻿using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Text;

namespace BTCMarketsBot
{
    public class BTCMarketsHelper
    {
        public static string ExchangeType { get; set; }

        /// <summary>
        /// Profit margin in percentage
        /// </summary>
        public static int ProfitMargin { get; set; }

        public static MarketTickData MarketTickData { get; set; }

        #region Market Methods

        public static void GetMarketTick()
        {
            MarketTickData = JsonConvert.DeserializeObject<MarketTickData>(SendRequest(MethodConstants.MARKET_TICK_PATH, null));
        }

        public static MarketTickData GetMarketTick(string pair)
        {
            return JsonConvert.DeserializeObject<MarketTickData>(SendRequest($"/market/{pair}/tick", null));
        }

        #endregion Market Methods

        #region Account Methods

        /// <summary>
        /// GET: Returns the current account balance of all Currency and BTC/LTC in your Account
        /// </summary>
        /// <returns>[{"balance":1000000000,"pendingFunds":0,"currency":"AUD"},{"balance":1000000000,"pendingFunds":0,"currency":"BTC"},{"balance":1000000000,"pendingFunds":0,"currency":"LTC"}]</returns>
        public static decimal RetrieveAccountBalance(string currency)
        {
            BalanceData items = new BalanceData(JsonConvert.DeserializeObject<BalanceItem[]>(SendRequest(MethodConstants.ACCOUNT_BALANCE_PATH, null)));

            return items.GetAvailableBalance(currency);
        }

        #endregion Account Methods

        #region Order History Methods

        /// <summary>
        /// POST: Requests Order History capped at a supplied limit, and for a specified timestamp (in ms)
        /// </summary>
        /// <param name="currency">AUD, USD</param>
        /// <param name="instrument">BTC/LTC etc</param>
        /// <param name="limit"># of Orders to return</param>
        /// <param name="since">Timestamp in ms to filter results</param>
        /// <returns>{"success":true,"errorCode":null,"errorMessage":null,"orders":[{"id":1003245675,"currency":"AUD","instrument":"BTC","orderSide":"Bid","ordertype":"Limit","creationTime":1378862733366,"status":"Placed","errorMessage":null,"price":13000000000,"volume":10000000,"openVolume":10000000,"clientRequestId":null,"trades":[]},{"id":4345675,"currency":"AUD","instrument":"BTC","orderSide":"Ask","ordertype":"Limit","creationTime":1378636912705,"status":"Fully Matched","errorMessage":null,"price":13000000000,"volume":10000000,"openVolume":0,"clientRequestId":null,"trades":[{"id":5345677,"creationTime":1378636913151,"description":null,"price":13000000000,"volume":10000000,"fee":100000}]}]}</returns>
        public static string OrderHistory(string currency, string instrument, int limit, string since)
        {
            return SendRequest(MethodConstants.ORDER_HISTORY_PATH, RequestHelper.BuildOrderString(currency, instrument, limit, since));
        }

        /// <summary>
        /// POST: Requests Order History but defaults in a timestamp of all and a limit of 10 orders
        /// </summary>
        /// <returns>{"success":true,"errorCode":null,"errorMessage":null,"orders":[{"id":1003245675,"currency":"AUD","instrument":"BTC","orderSide":"Bid","ordertype":"Limit","creationTime":1378862733366,"status":"Placed","errorMessage":null,"price":13000000000,"volume":10000000,"openVolume":10000000,"clientRequestId":null,"trades":[]},{"id":4345675,"currency":"AUD","instrument":"BTC","orderSide":"Ask","ordertype":"Limit","creationTime":1378636912705,"status":"Fully Matched","errorMessage":null,"price":13000000000,"volume":10000000,"openVolume":0,"clientRequestId":null,"trades":[{"id":5345677,"creationTime":1378636913151,"description":null,"price":13000000000,"volume":10000000,"fee":100000}]}]}</returns>
        public static string OrderHistory()
        {
            return SendRequest(MethodConstants.ORDER_HISTORY_PATH, RequestHelper.BuildDefaultOrderString());
        }

        /// <summary>
        /// POST: Requests Open Order History but defaults in a timestamp of all and a limit of 10 on all orders
        /// </summary>
        /// <param name="currency">AUD, USD</param>
        /// <param name="instrument">BTC/LTC etc</param>
        /// <param name="limit"># of Orders to return</param>
        /// <param name="since">Timestamp in ms to filter results</param>
        /// <returns>As per Order History response</returns>
        public static OrderHistoryData OrderOpen(string currency, string instrument, int limit, string since)
        {
            return JsonConvert.DeserializeObject<OrderHistoryData>(SendRequest(MethodConstants.ORDER_OPEN_PATH, RequestHelper.BuildOrderString(currency, instrument, limit, since)));
        }

        /// <summary>
        /// POST: Requests Open Order History but defaults in a timestamp of all and a limit of 10 on all orders
        /// </summary>
        /// <returns>As per Order History response</returns>
        public static OrderHistoryData OrderOpen()
        {
            return JsonConvert.DeserializeObject<OrderHistoryData>(SendRequest(MethodConstants.ORDER_OPEN_PATH, RequestHelper.BuildDefaultOrderString()));
        }

        /// <summary>
        /// POST: Requests All Trade History capped at a supplied limit, and for a specified timestamp (in ms)
        /// </summary>
        /// <param name="currency">AUD, USD</param>
        /// <param name="instrument">BTC/LTC etc</param>
        /// <param name="limit"># of Orders to return</param>
        /// <param name="since">Timestamp in ms to filter results</param>
        /// <returns></returns>
        public static string OrderTradeHistory(string currency, string instrument, int limit, string since)
        {
            return SendRequest(MethodConstants.ORDER_TRADE_HISTORY_PATH,
                RequestHelper.BuildOrderString(currency, instrument, limit, since));
        }

        /// <summary>
        /// POST: Requests All Trade History but defaults in a timestamp of all and a limit of 10 on all orders
        /// </summary>
        /// <returns></returns>
        public static string OrderTradeHistory()
        {
            return SendRequest(MethodConstants.ORDER_TRADE_HISTORY_PATH, RequestHelper.BuildDefaultOrderString());
        }

        #endregion Order History Methods

        #region Order Manipulation Methods

        /// <summary>
        /// POST: Gets all the Detail for an existing order in BTC Markets
        /// </summary>
        /// <param name="orderID">The unique ID of the orders details you want to view. Extend this to an array for multiple.</param>
        /// <returns>{"success":true,"errorCode":null,"errorMessage":null,"orders":[{"id":54345259,"currency":"AUD","instrument":"BTC","orderSide":"Ask","ordertype":"Limit","creationTime":1449022730366,"status":"Placed","errorMessage":null,"price":1000000000000,"volume":100000000,"openVolume":100000000,"clientRequestId":null,"trades":[]}]}</returns>
        public static string FetchOpenOrderDetail(string orderID)
        {
            return SendRequest(MethodConstants.ORDER_OPEN_DETAIL, RequestHelper.BuildOrderRequestFromID(orderID));
        }

        /// <summary>
        ///POST: Will Execute a new Order on BTC Markets
        /// </summary>
        /// <param name="currency">AUD, USD</param>
        /// <param name="instrument">BTC/LTC etc</param>
        /// <param name="price">How many coins to Buy or Sell</param>
        /// <param name="volume"># of Coins</param>
        /// <param name="orderSide">Bid (Buy) or Ask (Sell)</param>
        /// <param name="orderType">Limit or Market</param>
        /// <returns>Success: {"success":true,"errorCode":null,"errorMessage":null,"id":100,"clientRequestId":"abc-cdf-1000"}
        ///          Error: {"success":false,"errorCode":3,"errorMessage":"Invalid argument.","id":0,"clientRequestId":"abc-cdf-1000"}
        /// </returns>
        public static CreateOrderData CreateNewOrder(string currency, string instrument, long price, long volume,
            string orderSide, string orderType)
        {
            return JsonConvert.DeserializeObject<CreateOrderData>(SendRequest(
                MethodConstants.ORDER_CREATE_PATH,
                RequestHelper.BuildNewOrderString(currency, instrument, price, volume, orderSide, orderType)));
        }

        /// <summary>
        /// POST: Will Cancel an Order given a specific ID (multiple not currently supported but will be extended)
        /// </summary>
        /// <param name="orderID">The specific Order ID to cancel</param>
        /// <returns>{"success":true,"errorCode":null,"errorMessage":null,"responses":[{"success":false,"errorCode":3,"errorMessage":"order does not exist.","id":6840125478}]}</returns>
        public static string CancelOpenOrder(string orderID)
        {
            return SendRequest(MethodConstants.ORDER_OPEN_CANCEL, RequestHelper.BuildOrderRequestFromID(orderID));
        }

        #endregion Order Manipulation Methods

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
                Console.WriteLine(action);

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
}