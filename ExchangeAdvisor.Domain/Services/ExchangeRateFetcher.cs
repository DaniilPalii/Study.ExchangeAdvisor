using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public class ExchangeRateFetcher
    {
        public ExchangeRateFetcher(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<KeyValuePair<DateTime, double>[]> FetchHistoryAsync(
            DateTime startDate,
            DateTime endDate,
            CurrencySymbol buyingCurrencySymbol,
            CurrencySymbol sealingCurrencySymbol)
        {
            var response = await CreateHttpClient()
                .GetAsync(
                    $"history" +
                    $"?start_at={startDate:yyyy-MM-dd}" +
                    $"&end_at={endDate:yyyy-MM-dd}" +
                    $"&base={buyingCurrencySymbol}" +
                    $"&symbols={sealingCurrencySymbol}")
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

            return JsonSerializer.Deserialize<KeyValuePair<DateTime, double>[]>(responseContentString);
        }

        private HttpClient CreateHttpClient()
        {
            var client = httpClientFactory.CreateClient("Exchange Rates API");
            client.BaseAddress = new Uri("https://api.exchangeratesapi.io");

            return client;
        }

        private readonly IHttpClientFactory httpClientFactory;
    }
}