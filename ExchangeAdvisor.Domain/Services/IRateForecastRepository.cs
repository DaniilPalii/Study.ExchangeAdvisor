using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateForecastRepository
    {
        Task<bool> ExistsAsync(CurrencyPair currencyPair, DateTime creationDay);
        
        Task<RateForecast> GetAsync(CurrencyPair currencyPair, DateTime creationDay);

        Task<RateForecast> AddOrUpdateAsync(RateForecast forecast);
        
        Task<IEnumerable<RateForecastMetadata>> GetAllForecastsMetadataAsync(CurrencyPair currencyPair);
        
        Task<RateForecastMetadata> UpdateMetadataAsync(RateForecastMetadata metadata);
    }
}