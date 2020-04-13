using System;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IWebRateHistoryFetcher
    {
        Task<DateTime> GetLatestRateDate(CurrencyPair currencyPair);
        
        Task<RateHistory> FetchAsync(DateRange dateRange, CurrencyPair currencyPair);
    }
}