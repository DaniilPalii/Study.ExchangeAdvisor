using System;

namespace ExchangeAdvisor.Domain.Values.Rate
{
    public class RateForecastMetadata
    {
        public DateTime CreationDay { get; }

        public CurrencyPair CurrencyPair { get; }

        public string Description { get; set; }

        public RateForecastMetadata(DateTime creationDay, CurrencyPair currencyPair, string description)
            : this(creationDay, currencyPair)
        {
            Description = description;
        }

        public RateForecastMetadata(DateTime creationDay, CurrencyPair currencyPair)
        {
            CreationDay = creationDay;
            CurrencyPair = currencyPair;
        }
    }
}
