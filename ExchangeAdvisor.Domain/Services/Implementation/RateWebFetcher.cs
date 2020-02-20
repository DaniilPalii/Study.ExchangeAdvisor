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

        public async Task<IEnumerable<Rate>> FetchAsync(
            DateTime startDate,
            DateTime endDate,
            CurrencySymbol baseCurrencySymbol,
            CurrencySymbol comparingCurrencySymbol)
        {
            CheckDates(startDate, endDate);

            return await FetchRateHistoryAsync(
                "history"
                    + $"?start_at={startDate:yyyy-MM-dd}"
                    + $"&end_at={endDate:yyyy-MM-dd}"
                    + $"&base={baseCurrencySymbol}"
                    + $"&symbols={comparingCurrencySymbol}");
        }

        public async Task<IEnumerable<Rate>> FetchAsync(DateTime startDate, DateTime endDate, CurrencySymbol baseCurrencySymbol)
        {
            CheckDates(startDate, endDate);

            return await FetchRateHistoryAsync(
                "history"
                    + $"?start_at={startDate:yyyy-MM-dd}"
                    + $"&end_at={endDate:yyyy-MM-dd}"
                    + $"&base={baseCurrencySymbol}");
        }

        public async Task<IEnumerable<Rate>> FetchAsync(DateTime startDate, DateTime endDate)
        {
            CheckDates(startDate, endDate);

            return await FetchRateHistoryAsync(
                "history"
                    + $"?start_at={startDate:yyyy-MM-dd}"
                    + $"&end_at={endDate:yyyy-MM-dd}");
        }

        private static void CheckDates(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date should be greater or equal to start date");
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
                        value: rbc.Value,
                        baseCurrency: ratesHistoryResponse.@base.Value,
                        comparingCurrency: rbc.Key));
                })
                .OrderBy(r => r.Day)
                .ThenBy(r => r.BaseCurrency)
                .ThenBy(r => r.ComparingCurrency);
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
            public IDictionary<DateTime, IDictionary<CurrencySymbol, float>> rates { get; set; }
            public DateTime? start_at { get; set; }
            public DateTime? end_at { get; set; }
            public CurrencySymbol? @base { get; set; }
        }

        private readonly IHttpClientFactory httpClientFactory;
    }
}