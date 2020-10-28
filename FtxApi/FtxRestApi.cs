using System;
using DalSoft.RestClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FtxApi.Enums;
using System.Reflection;

namespace FtxApi
{
    public class FtxRestApi
    {
        private const string Url = "https://ftx.com/";

        private readonly Client _client;

        private readonly Config _restConfig;
        private readonly dynamic _restClient;

        private readonly HMACSHA256 _hashMaker;

        private long _nonce;
      
        public FtxRestApi(Client client)
        {
            _client = client;
            _restConfig = new Config
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _restClient = new RestClient(Url, _restConfig);

            _hashMaker = new HMACSHA256(Encoding.UTF8.GetBytes(_client.ApiSecret));
        }

        #region Coins

        public async Task<dynamic> GetCoinsAsync()
        {
            var resultString = $"api/coins";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        #endregion

        #region Futures

        public async Task<dynamic> GetAllFuturesAsync()
        {
            var resultString = $"api/futures";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        public async Task<dynamic> GetFutureAsync(string future)
        {
            var resultString = $"api/futures/{future}";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        public async Task<dynamic> GetFutureStatsAsync(string future)
        {
            var resultString = $"api/futures/{future}/stats";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        public async Task<dynamic> GetFundingRatesAsync(DateTime start, DateTime end)
        {
            var resultString = $"api/funding_rates?start_time={Util.Util.GetSecondsFromEpochStart(start)}&end_time={Util.Util.GetSecondsFromEpochStart(end)}";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        public async Task<dynamic> GetFundingRatesAsync()
        {
            var resultString = $"api/funding_rates";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        public async Task<dynamic> GetHistoricalPricesAsync(string futureName, int resolution, int limit, DateTime start, DateTime end)
        {
            var resultString = $"api/futures/{futureName}/mark_candles?resolution={resolution}&limit={limit}&start_time={Util.Util.GetSecondsFromEpochStart(start)}&end_time={Util.Util.GetSecondsFromEpochStart(end)}";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        #endregion

        #region Markets

        public async Task<dynamic> GetMarketsAsync()
        {
            var resultString = $"api/markets";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        public async Task<dynamic> GetSingleMarketsAsync(string marketName)
        {
            var resultString = $"api/markets/{marketName}";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        public async Task<dynamic> GetMarketOrderBookAsync(string marketName, int depth = 20)
        {
            var resultString = $"api/markets/{marketName}/orderbook?depth={depth}";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }
        
        public async Task<dynamic> GetMarketTradesAsync(string marketName, int limit, DateTime start, DateTime end)
        {
            var resultString = $"api/markets/{marketName}/trades?limit={limit}&start_time={Util.Util.GetSecondsFromEpochStart(start)}&end_time={Util.Util.GetSecondsFromEpochStart(end)}";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        #endregion

        #region Account
        
        public async Task<dynamic> GetAccountInfoAsync()
        {
            var resultString = $"api/account";
            var sign = GenerateSignature(HttpMethod.GET, "/api/account", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }


        public async Task<dynamic> GetPositionsAsync()
        {
            var resultString = $"api/positions";
            var sign = GenerateSignature(HttpMethod.GET, "/api/positions", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> ChangeAccountLeverageAsync(int leverage)
        {
            var resultString = $"api/account/leverage";
         
            var body = $"{{\"leverage\": {leverage}}}";

            var sign = GenerateSignature(HttpMethod.POST, "/api/account/leverage", body);

            var result = await CallAsyncSign(HttpMethod.POST, resultString, sign, body);

            return result;
        }

        #endregion

        #region Wallet

        public async Task<dynamic> GetCoinAsync()
        {
            var resultString = $"api/wallet/coins";
           
            var sign = GenerateSignature(HttpMethod.GET, "/api/wallet/coins", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> GetBalancesAsync()
        {
            var resultString = $"api/wallet/balances";

            var sign = GenerateSignature(HttpMethod.GET, "/api/wallet/balances", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> GetDepositAddressAsync(string coin)
        {
            var resultString = $"api/wallet/deposit_address/{coin}";
            
            var sign = GenerateSignature(HttpMethod.GET, $"/api/wallet/deposit_address/{coin}", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> GetDepositHistoryAsync()
        {
            var resultString = $"api/wallet/deposits";

            var sign = GenerateSignature(HttpMethod.GET, "/api/wallet/deposits", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> GetWithdrawalHistoryAsync()
        {
            var resultString = $"api/wallet/withdrawals";

            var sign = GenerateSignature(HttpMethod.GET, "/api/wallet/withdrawals", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> RequestWithdrawalAsync(string coin, decimal size, string addr, string tag, string pass, string code)
        {
            var resultString = $"api/wallet/withdrawals";

            var body = $"{{"+
                $"\"coin\": \"{coin}\"," +
                $"\"size\": {size},"+
                $"\"address\": \"{addr}\","+
                $"\"tag\": {tag},"+
                $"\"password\": \"{pass}\","+
                $"\"code\": {code}" +
                "}";

            var sign = GenerateSignature(HttpMethod.POST, "/api/wallet/withdrawals", body);

            var result = await CallAsyncSign(HttpMethod.POST, resultString, sign, body);

            return result;
        }

        #endregion

        #region Orders

        public async Task<dynamic> PlaceOrderAsync(string instrument, SideType side, decimal price, OrderType orderType, decimal amount, bool reduceOnly = false)
        {
            var path = $"api/orders";

            var body =
                $"{{\"market\": \"{instrument}\"," +
                $"\"side\": \"{side.ToString()}\"," +
                $"\"price\": {price}," +
                $"\"type\": \"{orderType.ToString()}\"," +
                $"\"size\": {amount}," +
                $"\"reduceOnly\": {reduceOnly.ToString().ToLower()}}}";

            var sign = GenerateSignature(HttpMethod.POST, "/api/orders", body);
            var result = await CallAsyncSign(HttpMethod.POST, path, sign, body);

            return result;
        }

        public async Task<dynamic> PlaceStopOrderAsync(string instrument, SideType side, decimal triggerPrice, decimal amount, bool reduceOnly = false)
        {
            var path = $"api/conditional_orders";

            var body =
                $"{{\"market\": \"{instrument}\"," +
                $"\"side\": \"{side.ToString()}\"," +
                $"\"triggerPrice\": {triggerPrice}," +
                $"\"type\": \"stop\"," +
                $"\"size\": {amount}," +
                $"\"reduceOnly\": {reduceOnly.ToString().ToLower()}}}";

            var sign = GenerateSignature(HttpMethod.POST, "/api/conditional_orders", body);
            var result = await CallAsyncSign(HttpMethod.POST, path, sign, body);

            return result;
        }

        public async Task<dynamic> PlaceTrailingStopOrderAsync(string instrument, SideType side, decimal trailValue, decimal amount, bool reduceOnly = false)
        {
            var path = $"api/conditional_orders";

            var body =
                $"{{\"market\": \"{instrument}\"," +
                $"\"side\": \"{side.ToString()}\"," +
                $"\"trailValue\": {trailValue}," +
                $"\"type\": \"trailingStop\"," +
                $"\"size\": {amount}," +
                $"\"reduceOnly\": {reduceOnly.ToString().ToLower()}}}";

            var sign = GenerateSignature(HttpMethod.POST, "/api/conditional_orders", body);
            var result = await CallAsyncSign(HttpMethod.POST, path, sign, body);

            return result;
        }

        public async Task<dynamic> PlaceTakeProfitOrderAsync(string instrument, SideType side, decimal triggerPrice, decimal amount, bool reduceOnly = false)
        {
            var path = $"api/conditional_orders";

            var body =
                $"{{\"market\": \"{instrument}\"," +
                $"\"side\": \"{side.ToString()}\"," +
                $"\"triggerPrice\": {triggerPrice}," +
                $"\"type\": \"takeProfit\"," +
                $"\"size\": {amount}," +
                $"\"reduceOnly\": {reduceOnly.ToString().ToLower()}}}";

            var sign = GenerateSignature(HttpMethod.POST, "/api/conditional_orders", body);
            var result = await CallAsyncSign(HttpMethod.POST, path, sign, body);

            return result;
        }

        public async Task<dynamic> GetOpenOrdersAsync(string instrument)
        {
            var path = $"api/orders?market={instrument}";

            var sign = GenerateSignature(HttpMethod.GET, $"/api/orders?market={instrument}", "");
           
            var result = await CallAsyncSign(HttpMethod.GET, path, sign);

            return result;
        }

        public async Task<dynamic> GetOrderStatusAsync(string id)
        {
            var resultString = $"api/orders/{id}";

            var sign = GenerateSignature(HttpMethod.GET, $"/api/orders/{id}", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> GetOrderStatusByClientIdAsync(string clientOrderId)
        {
            var resultString = $"api/orders/by_client_id/{clientOrderId}";

            var sign = GenerateSignature(HttpMethod.GET, $"/api/orders/by_client_id/{clientOrderId}", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> CancelOrderAsync(string id)
        {
            var resultString = $"api/orders/{id}";

            var sign = GenerateSignature(HttpMethod.DELETE, $"/api/orders/{id}", "");

            var result = await CallAsyncSign(HttpMethod.DELETE, resultString, sign);

            return result;
        }

        public async Task<dynamic> CancelOrderByClientIdAsync(string clientOrderId)
        {
            var resultString = $"api/orders/by_client_id/{clientOrderId}";

            var sign = GenerateSignature(HttpMethod.DELETE, $"/api/orders/by_client_id/{clientOrderId}", "");

            var result = await CallAsyncSign(HttpMethod.DELETE, resultString, sign);

            return result;
        }

        public async Task<dynamic> CancelAllOrdersAsync(string instrument)
        {
            var resultString = $"api/orders";

            var body =
                $"{{\"market\": \"{instrument}\"}}";

                var sign = GenerateSignature(HttpMethod.DELETE, $"/api/orders", body);

            var result = await CallAsyncSign(HttpMethod.DELETE, resultString, sign, body);

            return result;
        }

        #endregion

        #region Fills

        public async Task<dynamic> GetFillsAsync(string market, int limit, DateTime start, DateTime end)
        {
            var resultString = $"api/fills?market={market}&limit={limit}&start_time={Util.Util.GetSecondsFromEpochStart(start)}&end_time={Util.Util.GetSecondsFromEpochStart(end)}";

            var sign = GenerateSignature(HttpMethod.GET, $"/{resultString}", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        #endregion

        #region Funding

        public async Task<dynamic> GetFundingPaymentAsync(DateTime start, DateTime end)
        {
            var resultString = $"api/funding_payments?start_time={Util.Util.GetSecondsFromEpochStart(start)}&end_time={Util.Util.GetSecondsFromEpochStart(end)}";

            var sign = GenerateSignature(HttpMethod.GET, $"/{resultString}", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        #endregion

        #region Leveraged Tokens

        public async Task<dynamic> GetLeveragedTokensListAsync()
        {
            var resultString = $"api/lt/tokens";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        public async Task<dynamic> GetTokenInfoAsync(string tokenName)
        {
            var resultString = $"api/lt/{tokenName}";

            var result = await CallAsync(HttpMethod.GET, resultString);

            return result;
        }

        public async Task<dynamic> GetLeveragedTokenBalancesAsync()
        {
            var resultString = $"api/lt/balances";

            var sign = GenerateSignature(HttpMethod.GET, $"/api/lt/balances", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> GetLeveragedTokenCreationListAsync()
        {
            var resultString = $"api/lt/creations";

            var sign = GenerateSignature(HttpMethod.GET, $"/api/lt/creations", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> RequestLeveragedTokenCreationAsync(string tokenName, decimal size)
        {
            var resultString = $"api/lt/{tokenName}/create";
          
            var body = $"{{\"size\": {size}}}";

            var sign = GenerateSignature(HttpMethod.POST, $"/api/lt/{tokenName}/create", body);

            var result = await CallAsyncSign(HttpMethod.POST, resultString, sign, body);

            return result;
        }

        public async Task<dynamic> GetLeveragedTokenRedemptionListAsync()
        {
            var resultString = $"api/lt/redemptions";

            var sign = GenerateSignature(HttpMethod.GET, $"/api/lt/redemptions", "");

            var result = await CallAsyncSign(HttpMethod.GET, resultString, sign);

            return result;
        }

        public async Task<dynamic> RequestLeveragedTokenRedemptionAsync(string tokenName, decimal size)
        {
            var resultString = $"api/lt/{tokenName}/redeem";

            var body = $"{{\"size\": {size}}}";

            var sign = GenerateSignature(HttpMethod.POST, $"/api/lt/{tokenName}/redeem", body);

            var result = await CallAsyncSign(HttpMethod.POST, resultString, sign, body);

            return result;
        }

        #endregion

        #region Util
        private async Task<dynamic> CallAsync(HttpMethod method, string endpoint, string body = null)
        {
            switch (method)
            {
                case HttpMethod.DELETE:
                    return await _restClient.Resource(endpoint).Delete().ConfigureAwait(false);
                case HttpMethod.GET:
                    return await _restClient.Resource(endpoint).Get().ConfigureAwait(false);
                case HttpMethod.HEAD:
                    return await _restClient.Resource(endpoint).Head().ConfigureAwait(false);
                case HttpMethod.OPTIONS:
                    return await _restClient.Resource(endpoint).Options().ConfigureAwait(false);
                case HttpMethod.PATCH:
                    return await _restClient.Resource(endpoint).Patch().ConfigureAwait(false);
                case HttpMethod.POST:
                    return await _restClient.Resource(endpoint).Post().ConfigureAwait(false);
                case HttpMethod.PUT:
                    return await _restClient.Resource(endpoint).Put().ConfigureAwait(false);
                case HttpMethod.TRACE:
                    return await _restClient.Resource(endpoint).Trace().ConfigureAwait(false);
            }
            throw new ArgumentException("HttpMethod not valid");
        }

        private async Task<string> CallAsyncSign(HttpMethod method, string endpoint, string sign, string body = null)
        {
            /*
            var request = new HttpRequestMessage(method, endpoint);

            if (body != null)
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            request.Headers.Add("FTX-KEY", _client.ApiKey);
            request.Headers.Add("FTX-SIGN", sign);
            request.Headers.Add("FTX-TS", _nonce.ToString());

            var response = await _restClient.SendAsync(request).ConfigureAwait(false);

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return result;
            */
            throw new NotImplementedException();
        }

        private string GenerateSignature(HttpMethod method, string url, string requestBody)
        {
            _nonce = GetNonce();
            var signature = $"{_nonce}{method.ToString().ToUpper()}{url}{requestBody}";
            var hash = _hashMaker.ComputeHash(Encoding.UTF8.GetBytes(signature));
            var hashStringBase64 = BitConverter.ToString(hash).Replace("-", string.Empty);
            return hashStringBase64.ToLower();
        }

        private long GetNonce()
        {
            return Util.Util.GetMillisecondsFromEpochStart();
        }
        #endregion
    }
}