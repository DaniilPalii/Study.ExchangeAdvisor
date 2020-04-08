using ExchangeAdvisor.Domain.Values;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateService
    {
        Task<RateHistory> GetHistoryAsync(CurrencyPair currencyPair);

        Task<RateForecast> GetActualForecastAsync(CurrencyPair currencyPair);

        Task<RateForecast> GetSavedForecastAsync(CurrencyPair currencyPair, DateTime creationDay);

        Task<IEnumerable<RateForecastMetadata>> GetAllSavedForecastsMetadataAsync(CurrencyPair currencyPair);

        Task<RateForecastMetadata> UpdateForecastMetadataAsync(RateForecastMetadata modifiedDescription);
    }
}