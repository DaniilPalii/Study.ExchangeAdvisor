using System;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateHistoryRepository
    {
        Task<bool> ExistsAsync(CurrencyPair currencyPair);

        Task<DateTime> GetLastDayAsync(CurrencyPair currencyPair);
        
        Task<RateHistory> GetAsync(CurrencyPair currencyPair, DateRange dateRange);

        Task<RateHistory> GetAsync(CurrencyPair currencyPair);

        Task AddOrUpdateAsync(RateHistory history);
    }
}
