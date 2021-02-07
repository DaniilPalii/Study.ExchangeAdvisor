using System;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.SignalRClient.ViewModels
{
    public class ForecastMetadataTableRowViewModel : RateForecastMetadata
    {
        public bool IsSelected { get; set; }

        public ForecastMetadataTableRowViewModel(RateForecastMetadata rateForecastMetadata)
            : base(
                rateForecastMetadata.CurrencyPair,
                rateForecastMetadata.CreationDay,
                rateForecastMetadata.Description)
        { }

        public ForecastMetadataTableRowViewModel(CurrencyPair currencyPair, DateTime creationDay, string description)
            : base(currencyPair, creationDay, description)
        { }

        public ForecastMetadataTableRowViewModel(CurrencyPair currencyPair, DateTime creationDay)
            : base(currencyPair, creationDay)
        { }
    }
}