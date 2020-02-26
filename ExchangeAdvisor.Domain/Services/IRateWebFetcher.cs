using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateWebFetcher
    {
        Task<IEnumerable<Rate>> FetchAsync(DateRange dateRange, CurrencyPair currencyPair);

        Task<IEnumerable<Rate>> FetchAsync(DateRange dateRange, Currency baseCurrency);

        Task<IEnumerable<Rate>> FetchAsync(DateRange dateRange);
    }
}