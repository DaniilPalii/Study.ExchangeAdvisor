using System;
using System.Linq;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using DomainRateForecast = ExchangeAdvisor.Domain.Values.Rate.RateForecast;

namespace ExchangeAdvisor.DB.Entities
{
    public class RateForecast : RateCollectionEntityBase
    {
        public DateTime CreationDay { get; set; }

        public string Description { get; set; }

        public DomainRateForecast ToDomainRateForecast()
        {
            var rates = Rates.Select(r => r.ToDomain());
            var metadata = ToDomainForecastMetadata();

            return new DomainRateForecast(rates, metadata);
        }

        public RateForecastMetadata ToDomainForecastMetadata()
        {
            var currencyPair = new CurrencyPair(BaseCurrency, ComparingCurrency);

            return new RateForecastMetadata(CreationDay, currencyPair, Description);
        }
    }
}