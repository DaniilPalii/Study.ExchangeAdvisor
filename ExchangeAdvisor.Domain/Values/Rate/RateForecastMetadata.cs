using System;

namespace ExchangeAdvisor.Domain.Values.Rate
{
    public class RateForecastMetadata
    {
        public CurrencyPair CurrencyPair { get; }

        public DateTime CreationDay { get; }

        public string Description { get; set; }

        public RateForecastMetadata(CurrencyPair currencyPair, DateTime creationDay, string description)
            : this(currencyPair, creationDay)
        {
            Description = description;
        }

        public RateForecastMetadata(CurrencyPair currencyPair, DateTime creationDay)
        {
            CreationDay = creationDay;
            CurrencyPair = currencyPair;
        }

        public override string ToString()
        {
            return $"{nameof(CurrencyPair)}: {CurrencyPair}, {nameof(CreationDay)}: {CreationDay:d}";
        }
    }
}
