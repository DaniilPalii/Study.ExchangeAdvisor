using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using Newtonsoft.Json;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class ExchangeRateFetcher : IExchangeRateFetcher
    {
        public ExchangeRateFetcher(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<RateOnDay>> FetchRateHistoryAsync(
            DateTime startDate,
            DateTime endDate,
            CurrencySymbol baseCurrencySymbol,
            CurrencySymbol comparingCurrencySymbol)
        {
            if (endDate < startDate) throw new ArgumentException("End date should be greater or equal to start date");
            
            var response = await CreateHttpClient()
                .GetAsync(
                    $"history" +
                    $"?start_at={startDate:yyyy-MM-dd}" +
                    $"&end_at={endDate:yyyy-MM-dd}" +
                    $"&base={baseCurrencySymbol}" +
                    $"&symbols={comparingCurrencySymbol}")
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Can't get data from https://api.exchangeratesapi.io. " +
                    $"Response code: {response.StatusCode}" +
                    $"Response message: {response.RequestMessage}");
            }
            var responseContentString = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var ratesHistoryResponse = JsonConvert.DeserializeObject<RatesHistoryResponse>(responseContentString);
            
            return ratesHistoryResponse?.rates
                ?.Select(r => new RateOnDay
                {
                    Day = r.Key,
                    Rate = r.Value.Values.Single()
                })
                .OrderBy(r => r.Day);
        }

        private HttpClient CreateHttpClient()
        {
            var client = httpClientFactory.CreateClient("Exchange Rates API");
            client.BaseAddress = new Uri("https://api.exchangeratesapi.io");

            return client;
        }

        private class RatesHistoryResponse
        {
            public IDictionary<DateTime, IDictionary<CurrencySymbol, double>> rates { get; set; }
            public DateTime? start_at { get; set; }
            public DateTime? end_at { get; set; }
            public CurrencySymbol? @base { get; set; }
        }

        private readonly IHttpClientFactory httpClientFactory;
    }
}