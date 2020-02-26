using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using Newtonsoft.Json;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class RateWebFetcher : IRateWebFetcher
    {
        public RateWebFetcher(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Rate>> FetchAsync(DateRange dateRange, CurrencyPair currencyPair)
        {
            return await FetchRateHistoryAsync(
                "history"
                    + $"?start_at={dateRange.Start:yyyy-MM-dd}"
                    + $"&end_at={dateRange.End:yyyy-MM-dd}"
                    + $"&base={currencyPair.Base}"
                    + $"&symbols={currencyPair.Comparing}");
        }

        public async Task<IEnumerable<Rate>> FetchAsync(DateRange dateRange, Currency baseCurrency)
        {
            return await FetchRateHistoryAsync(
                "history"
                    + $"?start_at={dateRange.Start:yyyy-MM-dd}"
                    + $"&end_at={dateRange.End:yyyy-MM-dd}"
                    + $"&base={baseCurrency}");
        }

        public async Task<IEnumerable<Rate>> FetchAsync(DateRange dateRange)
        {
            return await FetchRateHistoryAsync(
                "history"
                    + $"?start_at={dateRange.Start:yyyy-MM-dd}"
                    + $"&end_at={dateRange.End:yyyy-MM-dd}");
        }

        private async Task<IEnumerable<Rate>> FetchRateHistoryAsync(string requestUri)
        {
            var response = await CreateHttpClient().GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Can't get data from https://api.exchangeratesapi.io. "
                        + $"response code: {response.StatusCode}, "
                        + $"response message: {response.RequestMessage}.");
            }
            var responseContentString = await response.Content.ReadAsStringAsync();
            var ratesHistoryResponse = JsonConvert.DeserializeObject<RatesHistoryResponse>(responseContentString);

            return ratesHistoryResponse?.rates
                ?.SelectMany(dayRatesByCurrency =>
                {
                    var day = dayRatesByCurrency.Key;
                    var ratesByCurrency = dayRatesByCurrency.Value;

                    return ratesByCurrency.Select(rbc => new Rate(
                        day,
                        rbc.Value,
                        new CurrencyPair(
                            @base: ratesHistoryResponse.@base.Value,
                            comparing: rbc.Key)));
                })
                .OrderBy(r => r.Day)
                .ThenBy(r => r.CurrencyPair.Base)
                .ThenBy(r => r.CurrencyPair.Comparing);
        }

        private HttpClient CreateHttpClient()
        {
            var client = httpClientFactory.CreateClient("Exchange Rates API");
            client.BaseAddress = new Uri("https://api.exchangeratesapi.io");

            return client;
        }

        private class RatesHistoryResponse
        {
            // TODO: use atrybutes to fix names
            public IDictionary<DateTime, IDictionary<Currency, float>> rates { get; set; }
            public DateTime? start_at { get; set; }
            public DateTime? end_at { get; set; }
            public Currency? @base { get; set; }
        }

        private readonly IHttpClientFactory httpClientFactory;
    }
}