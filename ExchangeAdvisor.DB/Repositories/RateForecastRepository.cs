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

        public async Task<RateForecast> GetNewestAsync(CurrencyPair currencyPair, DateRange dateRange)
        {
            await using var dbc = CreateDatabaseContext();

            var maxCreationDay = await GetMaxCreationDay(dbc, currencyPair);
            var forecast = await dbc.RateForecasts.SingleOrDefaultAsync(EqualsBy(currencyPair, maxCreationDay));
            forecast.Rates = await dbc.ForecastedRates.Where(EqualsBy(forecast, dateRange)).ToArrayAsync();

            return forecast.ToRateForecast();
        }

        public async Task<RateForecast> GetNewestAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            var maxCreationDay = await GetMaxCreationDay(dbc, currencyPair);
            
            return (await GetForecastWithRatesAsync(dbc, currencyPair, maxCreationDay)).ToRateForecast();
        }

        public async Task<RateForecastMetadata> GetMetadataAsync(CurrencyPair currencyPair, DateTime creationDay)
        {
            await using var dbc = CreateDatabaseContext();

            return (await GetForecastWithoutRatesAsync(dbc, currencyPair, creationDay)).ToRateForecastMetadata();
        }

        // TODO: restore add or update to make code more safety
        public async Task AddAsync(RateForecast forecast)
        {
            await using var dbc = CreateDatabaseContext();
            
            if (await ExistsAsync(dbc, forecast.CurrencyPair, forecast.CreationDay))
                throw new InvalidOperationException($"Can't add duplicated forecast ({forecast.Metadata})");

            await dbc.RateForecasts.AddAsync(new RateForecastEntity(forecast));

            await dbc.SaveChangesAsync();
        }

        public async Task<IEnumerable<RateForecastMetadata>> GetMetadatasAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            return await dbc.RateForecasts.Where(EqualsBy(currencyPair))
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
            return await dbc.RateForecasts.AnyAsync(EqualsBy(currencyPair, creationDay));
        }

        private static async Task<RateForecastEntity> GetForecastWithoutRatesAsync(
            DatabaseContext dbc,
            CurrencyPair currencyPair,
            DateTime creationDay)
        {
            return await dbc.RateForecasts.SingleOrDefaultAsync(EqualsBy(currencyPair, creationDay));
        }

        private static Task<RateForecastEntity> GetForecastWithRatesAsync(
            DatabaseContext dbc,
            CurrencyPair currencyPair,
            DateTime creationDay)
        {
            return dbc.RateForecasts.Include(f => f.Rates).SingleOrDefaultAsync(EqualsBy(currencyPair, creationDay));
        }

        private static async Task<DateTime> GetMaxCreationDay(DatabaseContext dbc, CurrencyPair currencyPair)
        {
            return await dbc.RateForecasts.Where(EqualsBy(currencyPair)).MaxAsync(f => f.CreationDay);
        }

        private static Expression<Func<RateForecastEntity, bool>> EqualsBy(CurrencyPair currencyPair)
        {
            return f => f.BaseCurrency == currencyPair.Base
                && f.ComparingCurrency == currencyPair.Comparing;
        }

        private static Expression<Func<RateForecastEntity, bool>> EqualsBy(
            CurrencyPair currencyPair,
            DateTime creationDay)
        {
            return f => f.BaseCurrency == currencyPair.Base
                && f.ComparingCurrency == currencyPair.Comparing
                && f.CreationDay == creationDay;
        }

        private static Expression<Func<ForecastedRateEntity, bool>> EqualsBy(
            RateForecastEntity forecast,
            DateRange dateRange)
        {
            return r => r.Forecast.Id == forecast.Id
                && dateRange.Start <= r.Day.Date
                && dateRange.End >= r.Day.Date;
        }
    }
}