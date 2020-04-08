using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IWebRateHistoryFetcher
    {
        Task<RateHistory> FetchAsync(DateRange dateRange, CurrencyPair currencyPair);
    }
}