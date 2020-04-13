using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Exceptions;
using ExchangeAdvisor.Domain.Services.Implementation.Web.Response;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using Newtonsoft.Json;

namespace ExchangeAdvisor.Domain.Services.Implementation.Web
{
    public class WebRateHistoryFetcher : IWebRateHistoryFetcher
    {
        public WebRateHistoryFetcher(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [SuppressMessage(category: "ReSharper", checkId: "PossibleInvalidOperationException")]
        public async Task<DateTime> GetLatestRateDate(CurrencyPair currencyPair)
        {
            var requestUri = $"latest?base={currencyPair.Base}&symbols={currencyPair.Comparing}";
            var latestRateResponse = await GetByHttpAsync<LatestRateResponse>(requestUri);

            return latestRateResponse.Date.Value;
        }

        public async Task<RateHistory> FetchAsync(DateRange dateRange, CurrencyPair currencyPair)
        {
            var requestUri = "history"
                + $"?start_at={dateRange.Start:yyyy-MM-dd}"
                + $"&end_at={dateRange.End:yyyy-MM-dd}"
                + $"&base={currencyPair.Base}"
                + $"&symbols={currencyPair.Comparing}";
            var ratesHistoryResponse = await GetByHttpAsync<RatesHistoryResponse>(requestUri);

            return ratesHistoryResponse.ToRateHistory();
        }

        private async Task<T> GetByHttpAsync<T>(string requestUri)
        {
            var response = await CreateHttpClient().GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
                throw await CreateExternalApiExceptionAsync(response);
            
            return await ReadAndDeserializeAsync<T>(response);
        }

        private HttpClient CreateHttpClient()
        {
            var client = httpClientFactory.CreateClient(WebApiName);
            client.BaseAddress = new Uri(WebApiAddress);

            return client;
        }

        private static async Task<T> ReadAndDeserializeAsync<T>(HttpResponseMessage response)
        {
            var responseContentString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseContentString);
        }

        private static async Task<ExternalApiException> CreateExternalApiExceptionAsync(HttpResponseMessage response)
        {
            var errorResponse = await ReadAndDeserializeAsync<ErrorResponse>(response);

            return new ExternalApiException(
                apiName: $"{WebApiName} ({WebApiAddress})",
                operation: $"sending GET request ({response.RequestMessage.RequestUri})",
                reason: errorResponse.Message);
        }

        private readonly IHttpClientFactory httpClientFactory;
        
        private const string WebApiName = "Exchange Rates API";
        private const string WebApiAddress = "https://api.exchangeratesapi.io";
    }
}