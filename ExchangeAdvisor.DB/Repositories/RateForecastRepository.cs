using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExchangeAdvisor.DB.Context;
using ExchangeAdvisor.DB.Entities;
using ExchangeAdvisor.Domain.Helpers;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using Microsoft.EntityFrameworkCore;
using DomainRateForecast = ExchangeAdvisor.Domain.Values.Rate.RateForecast;

namespace ExchangeAdvisor.DB.Repositories
{
    public class RateForecastRepository : RepositoryBase, IRateForecastRepository
    {
        public RateForecastRepository(IDatabaseConnectionStringReader connectionStringReader)
            : base(connectionStringReader)
        { }

        public async Task<bool> ExistsAsync(CurrencyPair currencyPair, DateTime creationDay)
        {
            await using var dbc = CreateDatabaseContext();

            return await dbc.RateForecast.AnyAsync(By(currencyPair, creationDay));
        }

        public async Task<DomainRateForecast> GetAsync(CurrencyPair currencyPair, DateTime creationDay)
        {
            await using var dbc = CreateDatabaseContext();

            return (await GetForecastWithRatesAsync(dbc, currencyPair, creationDay)).ToRateForecast();
        }

        public async Task<DomainRateForecast> UpdateAsync(DomainRateForecast forecast)
        {
            await using var dbc = CreateDatabaseContext();

            var existingForecast = await GetForecastWithRatesAsync(dbc, forecast.CurrencyPair, forecast.CreationDay);
            var existingRateDays = existingForecast.Rates.Select(r => r.Day).ToHashSet();
            var newRates = forecast.Rates.Where(r => !existingRateDays.Contains(r.Day));

            existingForecast.Rates.Add(newRates.Select(r => new ForecastRateEntity(r, existingForecast)));

            dbc.Entry(existingForecast).Property(f => f.Rates).IsModified = true;
            await dbc.SaveChangesAsync();

            return (await GetForecastWithRatesAsync(dbc, forecast.CurrencyPair, forecast.CreationDay))
                .ToRateForecast();
        }

        public async Task<IEnumerable<RateForecastMetadata>> GetAllForecastsMetadataAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            return await dbc.RateForecast.Where(f => currencyPair.Equals(f.BaseCurrency, f.ComparingCurrency))
                .Select(f => f.ToRateForecastMetadata())
                .ToArrayAsync();
        }

        public async Task<RateForecastMetadata> UpdateMetadataAsync(RateForecastMetadata metadata)
        {
            await using var dbc = CreateDatabaseContext();

            var existingForecast = await dbc.RateForecast.SingleAsync(By(metadata.CurrencyPair, metadata.CreationDay));

            existingForecast.Description = metadata.Description;

            dbc.Entry(existingForecast).Property(m => m.Description).IsModified = true;
            await dbc.SaveChangesAsync();

            return (await GetForecastWithoutRatesAsync(metadata, dbc)).ToRateForecastMetadata();
        }
        
        private static async Task<RateForecastEntity> GetForecastWithoutRatesAsync(
            RateForecastMetadata metadata,
            DatabaseContext dbc)
        {
            return await dbc.RateForecast.SingleAsync(By(metadata.CurrencyPair, metadata.CreationDay));
        }

        private static Task<RateForecastEntity> GetForecastWithRatesAsync(
            DatabaseContext dbc,
            CurrencyPair currencyPair,
            DateTime creationDay)
        {
            return dbc.RateForecast.Include(f => f.Rates).SingleAsync(By(currencyPair, creationDay));
        }

        private static Expression<Func<RateForecastEntity, bool>> By(CurrencyPair currencyPair, DateTime creationDay)
        {
            return rateForecast => currencyPair.Equals(rateForecast.BaseCurrency, rateForecast.ComparingCurrency)
                && rateForecast.CreationDay == creationDay;
        }
    }
}