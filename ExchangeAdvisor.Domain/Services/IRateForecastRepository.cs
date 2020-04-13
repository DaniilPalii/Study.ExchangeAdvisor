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

        Task<RateForecast> GetNewestAsync(CurrencyPair currencyPair);

        Task<RateForecastMetadata> GetMetadataAsync(CurrencyPair currencyPair, DateTime creationDay);
        
        Task<IEnumerable<RateForecastMetadata>> GetMetadatasAsync(CurrencyPair currencyPair);

        Task AddAsync(RateForecast forecast);
        
        Task UpdateMetadataAsync(RateForecastMetadata metadata);
    }
}