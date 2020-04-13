using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExchangeAdvisor.DB.Context;
using ExchangeAdvisor.DB.Entities;
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

            return await ExistsAsync(dbc, currencyPair, creationDay);
        }

        public async Task<RateForecast> GetAsync(CurrencyPair currencyPair, DateTime creationDay)
        {
            await using var dbc = CreateDatabaseContext();

            return (await GetForecastWithRatesAsync(dbc, currencyPair, creationDay)).ToRateForecast();
        }

        public async Task<RateForecast> GetNewestAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            var maxCreationDay = await dbc.RateForecast.Where(EqualsBy(currencyPair)).MaxAsync(f => f.CreationDay);
            
            return (await GetForecastWithRatesAsync(dbc, currencyPair, maxCreationDay)).ToRateForecast();
        }

        public async Task<RateForecastMetadata> GetMetadataAsync(CurrencyPair currencyPair, DateTime creationDay)
        {
            await using var dbc = CreateDatabaseContext();

            return (await GetForecastWithoutRatesAsync(dbc, currencyPair, creationDay)).ToRateForecastMetadata();
        }

        public async Task AddAsync(RateForecast forecast)
        {
            await using var dbc = CreateDatabaseContext();
            
            if (await ExistsAsync(dbc, forecast.CurrencyPair, forecast.CreationDay))
                throw new InvalidOperationException($"Can't add duplicated forecast ({forecast.Metadata})");

            await dbc.RateForecast.AddAsync(new RateForecastEntity(forecast));

            await dbc.SaveChangesAsync();
        }

        public async Task<IEnumerable<RateForecastMetadata>> GetMetadatasAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            return await dbc.RateForecast.Where(EqualsBy(currencyPair))
                .Select(f => f.ToRateForecastMetadata())
                .ToArrayAsync();
        }

        public async Task UpdateMetadataAsync(RateForecastMetadata metadata)
        {
            await using var dbc = CreateDatabaseContext();

            var existingForecast = await GetForecastWithoutRatesAsync(dbc, metadata.CurrencyPair, metadata.CreationDay);

            existingForecast.Description = metadata.Description;

            dbc.Entry(existingForecast).Property(m => m.Description).IsModified = true;
            await dbc.SaveChangesAsync();
        }

        private static async Task<bool> ExistsAsync(
            DatabaseContext dbc,
            CurrencyPair currencyPair,
            DateTime creationDay)
        {
            return await dbc.RateForecast.AnyAsync(EqualsBy(currencyPair, creationDay));
        }

        private static async Task<RateForecastEntity> GetForecastWithoutRatesAsync(
            DatabaseContext dbc,
            CurrencyPair currencyPair,
            DateTime creationDay)
        {
            return await dbc.RateForecast.SingleOrDefaultAsync(EqualsBy(currencyPair, creationDay));
        }

        private static Task<RateForecastEntity> GetForecastWithRatesAsync(
            DatabaseContext dbc,
            CurrencyPair currencyPair,
            DateTime creationDay)
        {
            return dbc.RateForecast.Include(f => f.Rates).SingleOrDefaultAsync(EqualsBy(currencyPair, creationDay));
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