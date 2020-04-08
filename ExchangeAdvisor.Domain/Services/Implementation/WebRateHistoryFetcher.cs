using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using Newtonsoft.Json;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class WebRateHistoryFetcher : IWebRateHistoryFetcher
    {
        public WebRateHistoryFetcher(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<RateHistory> FetchAsync(DateRange dateRange, CurrencyPair currencyPair)
        {
            var requestUri = CreateGetHistoryUrl(dateRange, currencyPair);
            var response = await CreateHttpClient().GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
                throw CreateHttpRequestException(response);

            var responseContentString = await response.Content.ReadAsStringAsync();
            var ratesHistoryResponse = JsonConvert.DeserializeObject<RatesHistoryResponse>(responseContentString);

            return ToRateHistory(ratesHistoryResponse);
        }

        private HttpClient CreateHttpClient()
        {
            var client = httpClientFactory.CreateClient(name: "Exchange Rates API");
            client.BaseAddress = new Uri("https://api.exchangeratesapi.io");

            return client;
        }

        private static string CreateGetHistoryUrl(DateRange dateRange, CurrencyPair currencyPair)
        {
            return "history"
                + $"?start_at={dateRange.Start:yyyy-MM-dd}"
                + $"&end_at={dateRange.End:yyyy-MM-dd}"
                + $"&base={currencyPair.Base}"
                + $"&symbols={currencyPair.Comparing}";
        }

        private static HttpRequestException CreateHttpRequestException(HttpResponseMessage response)
        {
            return new HttpRequestException(
                "Can't get data from https://api.exchangeratesapi.io. "
                    + $"response code: {response.StatusCode}, "
                    + $"response message: {response.RequestMessage}.");
        }

        [SuppressMessage(category: "ReSharper", checkId: "PossibleInvalidOperationException")]
        private static RateHistory ToRateHistory(RatesHistoryResponse ratesHistoryResponse)
        {
            var rates = ratesHistoryResponse.Rates.Select(ToRate);
            
            var baseCurrency = ratesHistoryResponse.BaseCurrency.Value;
            var comparingCurrency = GetSingleComparingCurrency(ratesHistoryResponse.Rates);
            
            return new RateHistory(rates, new CurrencyPair(baseCurrency, comparingCurrency));
        }

        private static Rate ToRate(KeyValuePair<DateTime, IDictionary<Currency, float>> rateValueByCurrencyByDay)
        {
            var day = rateValueByCurrencyByDay.Key;
            
            var rateValueByCurrency = rateValueByCurrencyByDay.Value.Single();
            var rateValue = rateValueByCurrency.Value;

            return new Rate(day, rateValue);
        }

        private static Currency GetSingleComparingCurrency(
            IDictionary<DateTime,IDictionary<Currency,float>> rateValuesByCurrencyByDay)
        {
            var firstComparingCurrency = rateValuesByCurrencyByDay.First().Value.Single().Key;
            
            if (rateValuesByCurrencyByDay.Skip(1).Any(x => x.Value.Single().Key != firstComparingCurrency))
                throw new ArgumentException("There are different currencies");

            return firstComparingCurrency;
        }

        private class RatesHistoryResponse
        {
            [JsonProperty(propertyName: "rates")]
            public IDictionary<DateTime, IDictionary<Currency, float>> Rates { get; set; }
            
            [JsonProperty(propertyName: "start_at")]
            public DateTime? StartAt { get; set; }
            
            [JsonProperty(propertyName: "end_at")]
            public DateTime? EndAt { get; set; }
            
            [JsonProperty(propertyName: "base")]
            public Currency? BaseCurrency { get; set; }
        }

        private readonly IHttpClientFactory httpClientFactory;
    }
}