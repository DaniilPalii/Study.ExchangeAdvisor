using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using Newtonsoft.Json;

namespace ExchangeAdvisor.Domain.Services.Implementation.Web.Response
{
    public class RatesHistoryResponse
    {
        [JsonProperty(propertyName: "rates")]
        public IDictionary<DateTime, IDictionary<Currency, float>> Rates { get; set; }

        [JsonProperty(propertyName: "start_at")]
        public DateTime? StartAt { get; set; }

        [JsonProperty(propertyName: "end_at")]
        public DateTime? EndAt { get; set; }

        [JsonProperty(propertyName: "base")]
        public Currency? BaseCurrency { get; set; }

        [SuppressMessage(category: "ReSharper", checkId: "PossibleInvalidOperationException")]
        public RateHistory ToRateHistory()
        {
            var rates = Rates.Select(ToRate);
                
            var baseCurrency = BaseCurrency.Value;
            var comparingCurrency = GetSingleComparingCurrency(Rates);
            
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
            IDictionary<DateTime, IDictionary<Currency, float>> rateValuesByCurrencyByDay)
        {
            var firstComparingCurrency = rateValuesByCurrencyByDay.First().Value.Single().Key;
            
            if (rateValuesByCurrencyByDay.Skip(count: 1).Any(x => x.Value.Single().Key != firstComparingCurrency))
                throw new ArgumentException(message: "There are different currencies");

            return firstComparingCurrency;
        }
    }
}