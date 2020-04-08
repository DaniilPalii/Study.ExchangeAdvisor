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

            return await dbc.RateForecast.AnyAsync(EqualsBy(currencyPair, creationDay));
        }

        public async Task<RateForecast> GetAsync(CurrencyPair currencyPair, DateTime creationDay)
        {
            await using var dbc = CreateDatabaseContext();

            return (await GetForecastWithRatesAsync(dbc, currencyPair, creationDay)).ToRateForecast();
        }

        public async Task<RateForecast> AddOrUpdateAsync(RateForecast forecast)
        {
            await using var dbc = CreateDatabaseContext();

            var existingForecast = await GetForecastWithRatesAsync(dbc, forecast.CurrencyPair, forecast.CreationDay);

            if (existingForecast == null)
                dbc.RateForecast.Add(new RateForecastEntity(forecast));
            else
                Update(dbc, existingForecast, forecast);

            await dbc.SaveChangesAsync();

            return (await GetForecastWithRatesAsync(dbc, forecast.CurrencyPair, forecast.CreationDay)).ToRateForecast();
        }

        public async Task<IEnumerable<RateForecastMetadata>> GetAllForecastsMetadataAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            return await dbc.RateForecast.Where(EqualsBy(currencyPair))
                .Select(f => f.ToRateForecastMetadata())
                .ToArrayAsync();
        }

        public async Task<RateForecastMetadata> UpdateMetadataAsync(RateForecastMetadata metadata)
        {
            await using var dbc = CreateDatabaseContext();

            var existingForecast = await GetForecastWithoutRatesAsync(dbc, metadata);

            existingForecast.Description = metadata.Description;

            dbc.Entry(existingForecast).Property(m => m.Description).IsModified = true;
            await dbc.SaveChangesAsync();

            return (await GetForecastWithoutRatesAsync(dbc, metadata)).ToRateForecastMetadata();
        }

        private static async Task<RateForecastEntity> GetForecastWithoutRatesAsync(
            DatabaseContext dbc,
            RateForecastMetadata metadata)
        {
            return await dbc.RateForecast.SingleOrDefaultAsync(EqualsBy(metadata.CurrencyPair, metadata.CreationDay));
        }

        private static Task<RateForecastEntity> GetForecastWithRatesAsync(
            DatabaseContext dbc,
            CurrencyPair currencyPair,
            DateTime creationDay)
        {
            return dbc.RateForecast.Include(f => f.Rates).SingleOrDefaultAsync(EqualsBy(currencyPair, creationDay));
        }

        private static void Update(DatabaseContext dbc, RateForecastEntity existingForecast, RateForecast newForecast)
        {
            var existingRateDays = existingForecast.Rates.Select(r => r.Day).ToHashSet();
            var newRates = newForecast.Rates.Where(r => !existingRateDays.Contains(r.Day));

            existingForecast.Rates.Add(newRates.Select(r => new ForecastRateEntity(r, existingForecast)));

            dbc.Entry(existingForecast).Collection(f => f.Rates).IsModified = true;
        }

        private static Expression<Func<RateForecastEntity, bool>> EqualsBy(CurrencyPair currencyPair)
        {
            return forecast => forecast.BaseCurrency == currencyPair.Base
                && forecast.ComparingCurrency == currencyPair.Comparing;
        }

        private static Expression<Func<RateForecastEntity, bool>> EqualsBy(
            CurrencyPair currencyPair,
            DateTime creationDay)
        {
            return forecast => forecast.BaseCurrency == currencyPair.Base
                && forecast.ComparingCurrency == currencyPair.Comparing
                && forecast.CreationDay == creationDay;
        }
    }
}