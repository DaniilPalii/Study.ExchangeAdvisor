using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExchangeAdvisor.DB.Context;
using ExchangeAdvisor.DB.Entities;
using ExchangeAdvisor.Domain.Extensions;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using Microsoft.EntityFrameworkCore;

namespace ExchangeAdvisor.DB.Repositories
{
    public class RateHistoryRepository : RepositoryBase, IRateHistoryRepository
    {
        public RateHistoryRepository(IDatabaseConnectionStringReader connectionStringReader) : base(
            connectionStringReader) { }

        public async Task<bool> ExistsAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            return await dbc.RateHistories.AnyAsync(EqualsBy(currencyPair));
        }

        public async Task<DateTime> GetLastDayAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            var history = await GetHistoryWithoutRatesAsync(dbc, currencyPair);

            return await dbc.HistoricalRates.Where(r => r.History.Id == history.Id).MaxAsync(r => r.Day);
        }

        public async Task<RateHistory> GetAsync(CurrencyPair currencyPair, DateRange dateRange)
        {
            await using var dbc = CreateDatabaseContext();

            var history = await GetHistoryWithoutRatesAsync(dbc, currencyPair);
            history.Rates = await dbc.HistoricalRates.Where(EqualsBy(history, dateRange)).ToArrayAsync();

            return history.ToRateHistory();
        }

        public async Task<RateHistory> GetAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            return (await GetHistoryWithRatesAsync(dbc, currencyPair)).ToRateHistory();
        }

        public async Task AddOrUpdateAsync(RateHistory history)
        {
            await using var dbc = CreateDatabaseContext();

            var existingHistory = await GetHistoryWithRatesAsync(dbc, history.CurrencyPair);

            if (existingHistory == null)
                await dbc.RateHistories.AddAsync(new RateHistoryEntity(history));
            else
                Update(dbc, existingHistory, history);

            await dbc.SaveChangesAsync();
        }

        private static Task<RateHistoryEntity> GetHistoryWithRatesAsync(DatabaseContext dbc, CurrencyPair currencyPair)
        {
            return dbc.RateHistories.Include(h => h.Rates).SingleOrDefaultAsync(EqualsBy(currencyPair));
        }

        private static Task<RateHistoryEntity> GetHistoryWithoutRatesAsync(
            DatabaseContext dbc,
            CurrencyPair currencyPair)
        {
            return dbc.RateHistories.SingleOrDefaultAsync(EqualsBy(currencyPair));
        }

        private static void Update(DatabaseContext dbc, RateHistoryEntity existingHistory, RateHistory newHistory)
        {
            var existingRateDays = existingHistory.Rates.Select(r => r.Day).ToHashSet();
            var newRates = newHistory.Rates.Where(r => !existingRateDays.Contains(r.Day));

            existingHistory.Rates.Add(newRates.Select(r => new HistoricalRateEntity(r, existingHistory)));

            dbc.Entry(existingHistory).Collection(h => h.Rates).IsModified = true;
        }

        private static Expression<Func<RateHistoryEntity, bool>> EqualsBy(CurrencyPair currencyPair)
        {
            return h => h.BaseCurrency == currencyPair.Base
                && h.ComparingCurrency == currencyPair.Comparing;
        }

        private Expression<Func<HistoricalRateEntity, bool>> EqualsBy(RateHistoryEntity history, DateRange dateRange)
        {
            return r => r.History.Id == history.Id
                && dateRange.Start <= r.Day.Date
                && dateRange.End >= r.Day.Date;
        }
    }
}